﻿using Confluent.Kafka;
using System.Text.RegularExpressions;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KafkaLogEnricher
{
    public sealed class KafkaLogEnricher
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private IConsumer<Ignore, string> first_consumer = null;
        private IProducer<Null, string> second_producer = null;

        public KafkaLogEnricher(IConfiguration configuration, ILogger<KafkaLogEnricher> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task EnricherMain(CancellationToken cancellationToken)
        {
            do
            {
                try
                {
                    var procedureName = SharedConstants.SP_TopicTrace;
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@State", SqlDbType.Int) { Value = (int)TopicState.ShowData },
                    };
                    DataSet dataSet = await SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, sqlParameters);
                    if (dataSet.Tables != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                    {
                        DataTable dataTable = dataSet.Tables[0];
                        DataRow dataRow = dataTable.Rows[0];
                        SharedVariables.InputTopic = dataRow["FirstTopicName"] != DBNull.Value ? dataRow["FirstTopicName"].ToString() : SharedConstants.MagicString;
                        SharedVariables.OutputTopic = dataRow["SecondTopicName"] != DBNull.Value ? dataRow["SecondTopicName"].ToString() : SharedConstants.MagicString;
                        SharedVariables.IsInputTopicCreated = dataRow["isFirstTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isFirstTopicCreated"]) == 1 : false;
                        SharedVariables.IsOutputTopicCreated = dataRow["isSecondTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isSecondTopicCreated"]) == 1 : false;
                    }
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError($"Error updating last read position for file : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating last read position for file : {ex.Message}");
                }
            }
            while (!SharedVariables.IsInputTopicCreated); // Wait for the first topic to be created
            try
            {
                // Access values from appsettings.json
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
                first_consumer = new ConsumerBuilder<Ignore, string>(firstconsumerconfig).Build();
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

                second_producer = new ProducerBuilder<Null, string>(secondproducerconfig).Build();
                string ServiceName = SharedConstants.MagicString;
                bool isInsideService = false;
                string ServiceThreadId = SharedConstants.MagicString;
                SharedVariables.IsOutputTopicCreated = true;
                try
                {
                    var procedureName = SharedConstants.SP_TopicTrace;
                    SqlParameter[] parameters = new SqlParameter[]
                    {
                        new SqlParameter("@State", SqlDbType.Int) { Value = (int)TopicState.UpdateData },
                        new SqlParameter("@FirstTopicName", SharedVariables.InputTopic),
                        new SqlParameter("@SecondTopicName", SharedVariables.OutputTopic),
                        new SqlParameter("@isFirstTopicCreated", SharedVariables.IsInputTopicCreated ? 1 : 0),
                        new SqlParameter("@isSecondTopicCreated", SharedVariables.IsOutputTopicCreated ? 1 : 0)
                    };
                    await SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
                }
                catch (SqlException sqlEx)
                {
                    _logger.LogError($"Error updating last read position for file : {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error updating last read position for file : {ex.Message}");
                }

                _logger.LogInformation($"Output Topic :'{SharedVariables.OutputTopic}' data inserted successfully into DB");

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
                                await second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                            }
                            else if (isInsideService && string.Equals(ServiceThreadId, threadIdMatch.Groups[1].Value) && serviceEndMatch.Success && string.Equals(ServiceName, serviceEndMatch.Groups[3].Value))
                            {
                                await second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                                // _logger.LogInformation($"Published Service: {ServiceName} to Kafka");
                                isInsideService = false;
                            }
                            else if (serviceStartMatch.Success)
                            {
                                isInsideService = true;
                                ServiceName = serviceStartMatch.Groups[1].Value;
                                await second_producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult1.Value });
                                // Console.WriteLine($"Processed and published message to Kafka: {consumeResult.Value}");
                                if (threadIdMatch.Success)
                                {
                                    ServiceThreadId = threadIdMatch.Groups[1].Value;
                                }
                            }
                        }
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogError($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in EnricherMain: {ex.Message}");
            }
            _logger.LogInformation("Exiting Enricher Method...");
        }
    }
}
