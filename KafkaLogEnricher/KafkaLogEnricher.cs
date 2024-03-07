using Confluent.Kafka;
using System.Text.RegularExpressions;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace KafkaLogEnricher
{
    public class KafkaLogEnricher
    {
        private readonly IConfiguration _configuration;
        public KafkaLogEnricher(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task EnricherMain(CancellationToken cancellationToken)
        {
            // Access values from appsettings.json
            do
            {
                Console.WriteLine("Waiting for First Topic's data to be inserted...");
                Thread.Sleep(5000);
                try
                {
                    var query = "SELECT [FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated] FROM [SpiderETMDB].[dbo].[TopicTrace]";
                    DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(query, CommandType.Text);
                    if (dataTable != null)
                    {
                        DataRow dataRow = dataTable.Rows[0];
                        SharedVariables.InputTopic = dataRow["FirstTopicName"] != DBNull.Value ? dataRow["FirstTopicName"].ToString() : "";
                        SharedVariables.OutputTopic = dataRow["SecondTopicName"] != DBNull.Value ? dataRow["SecondTopicName"].ToString() : "";
                        SharedVariables.IsInputTopicCreated = dataRow["isFirstTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isFirstTopicCreated"]) == 1 : false;
                        SharedVariables.IsOutputTopicCreated = dataRow["isSecondTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isSecondTopicCreated"]) == 1 : false;
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"Error updating last read position for file : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating last read position for file : {ex.Message}");
                }
            }
            while (!SharedVariables.IsInputTopicCreated); // Wait for the first topic to be created
            try
            {
                var kafkaFirstConsumerConfig = _configuration.GetSection("FirstConsumerConfig");

                var firstconsumerconfig = new ConsumerConfig
                {
                    BootstrapServers = kafkaFirstConsumerConfig["BootstrapServers"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.ScramSha512,
                    SaslUsername = kafkaFirstConsumerConfig["SaslUsername"],
                    SaslPassword = kafkaFirstConsumerConfig["SaslPassword"],
                    SslCaLocation = kafkaFirstConsumerConfig["SslCaLocation"],
                    GroupId = kafkaFirstConsumerConfig["GroupID"],
                    AutoOffsetReset = (AutoOffsetReset)Enum.Parse(typeof(AutoOffsetReset), kafkaFirstConsumerConfig["AutoOffsetReset"]),
                    MaxPartitionFetchBytes = int.Parse(kafkaFirstConsumerConfig["MaxPartitionFetchBytes"]),
                    EnableAutoCommit = bool.Parse(kafkaFirstConsumerConfig["EnableAutoCommit"]),
                    AutoCommitIntervalMs = int.Parse(kafkaFirstConsumerConfig["AutoCommitIntervalMs"]),
                };

                // Create First Kafka consumer
                using var first_consumer = new ConsumerBuilder<Ignore, string>(firstconsumerconfig).Build();
                first_consumer.Subscribe(SharedVariables.InputTopic);

                // Create Second Kafka producer
                var kafkaSecondProducerConfig = _configuration.GetSection("SecondProducerConfig");

                var secondproducerconfig = new ProducerConfig
                {
                    BootstrapServers = kafkaSecondProducerConfig["BootstrapServers"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.ScramSha512,
                    SaslUsername = kafkaSecondProducerConfig["SaslUsername"],
                    SaslPassword = kafkaSecondProducerConfig["SaslPassword"],
                    SslCaLocation = kafkaSecondProducerConfig["SslCaLocation"],
                    CompressionType = CompressionType.Snappy,
                    MessageSendMaxRetries = int.Parse(kafkaSecondProducerConfig["MessageSendMaxRetries"]),
                    Acks = Acks.All
                };

                using var second_producer = new ProducerBuilder<Null, string>(secondproducerconfig).Build();
                string ServiceName = "";
                bool isInsideService = false;
                string ServiceThreadId = "";
                SharedVariables.IsOutputTopicCreated = true;
                try
                {
                    var query = "UPDATE [SpiderETMDB].[dbo].[TopicTrace] SET [FirstTopicName] = @FirstTopicName, [SecondTopicName] = @SecondTopicName, [isFirstTopicCreated] = @isFirstTopicCreated, [isSecondTopicCreated] = @isSecondTopicCreated";
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                    new SqlParameter("@FirstTopicName", SharedVariables.InputTopic),
                    new SqlParameter("@SecondTopicName", SharedVariables.OutputTopic),
                    new SqlParameter("@isFirstTopicCreated", SharedVariables.IsInputTopicCreated ? 1 : 0),
                    new SqlParameter("@isSecondTopicCreated", SharedVariables.IsOutputTopicCreated ? 1 : 0)
                    };
                    SqlDBHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"Error updating last read position for file : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating last read position for file : {ex.Message}");
                }

                Console.WriteLine($"Output Topic :'{SharedVariables.OutputTopic}' data inserted successfully into DB");

                // Process incoming messages
                while (true)
                {
                    try
                    {
                        var consumeResult1 = first_consumer.Consume();
                        if (consumeResult1 != null)
                        {
                            Match threadIdMatch = SharedConstants.ThreadIdRegex.Match(consumeResult1.Value);
                            Match serviceStartMatch = SharedConstants.ServiceStartRegex.Match(consumeResult1.Value);
                            Match serviceEndMatch = SharedConstants.ServiceEndRegex.Match(consumeResult1.Value);
                            if (isInsideService && !serviceEndMatch.Success)
                            {
                                second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                            }
                            else if (isInsideService && string.Equals(ServiceThreadId, threadIdMatch.Groups[1].Value) && serviceEndMatch.Success && string.Equals(ServiceName, serviceEndMatch.Groups[3].Value))
                            {
                                second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                                Console.WriteLine($"Published Service: {ServiceName} to Kafka");
                                isInsideService = false;
                            }
                            else if (serviceStartMatch.Success)
                            {
                                isInsideService = true;
                                ServiceName = serviceStartMatch.Groups[1].Value;
                                second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                                //Console.WriteLine($"Processed and published message to Kafka: {consumeResult.Value}");
                                if (threadIdMatch.Success)
                                {
                                    ServiceThreadId = threadIdMatch.Groups[1].Value;
                                }
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EnricherMain: {ex.Message}");
            }
        }
    }
}
