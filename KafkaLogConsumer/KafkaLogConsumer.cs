using Confluent.Kafka;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using KafkaClassLibrary;
using Microsoft.Extensions.Configuration;

namespace KafkaLogConsumer
{
    public class KafkaLogConsumer
    {
        private static int recordCounter = 0;
        private readonly IConfiguration _configuration;
        public KafkaLogConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConsumerMain()
        {
            do
            {
                Console.WriteLine("Waiting for Second Topic to be created...");
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
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while fetching record from DB : {ex.Message}");
                }
            }
            while (!SharedVariables.IsOutputTopicCreated); // Wait for the first topic to be created

            // Access values from appsettings.json
            var kafkaSecondConsumerConfig = _configuration.GetSection("SecondConsumerConfig");

            var secondconsumerconfig = new ConsumerConfig
            {
                BootstrapServers = kafkaSecondConsumerConfig["BootstrapServers"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.ScramSha512,
                SaslUsername = kafkaSecondConsumerConfig["SaslUsername"],
                SaslPassword = kafkaSecondConsumerConfig["SaslPassword"],
                SslCaLocation =kafkaSecondConsumerConfig["SslCaLocation"],
                GroupId = kafkaSecondConsumerConfig["GroupID"],
                AutoOffsetReset = (AutoOffsetReset)Enum.Parse(typeof(AutoOffsetReset), kafkaSecondConsumerConfig["AutoOffsetReset"]),
                MaxPartitionFetchBytes = int.Parse(kafkaSecondConsumerConfig["MaxPartitionFetchBytes"]),
                EnableAutoCommit = bool.Parse(kafkaSecondConsumerConfig["EnableAutoCommit"]),
                AutoCommitIntervalMs = int.Parse(kafkaSecondConsumerConfig["AutoCommitIntervalMs"])
            };

            // Create Kafka consumer
            using var second_consumer = new ConsumerBuilder<Ignore, string>(secondconsumerconfig).Build();

            // Subscribe to Kafka topic
            second_consumer.Subscribe(SharedVariables.OutputTopic);

            // Start consuming messages
            Console.WriteLine("Consuming messages from Output Topic to be inserted over DB...");
            while (true)
            {
                try
                {
                    var consumeResult2 = second_consumer.Consume();

                    if (consumeResult2 != null)
                    {
                        string serviceLogEntry = consumeResult2.Message.Value;
                        Match serviceStartMatch = SharedConstants.ServiceStartRegex.Match(serviceLogEntry);
                        Match serviceEndMatch = SharedConstants.ServiceEndRegex.Match(serviceLogEntry);

                        if (serviceStartMatch.Success)
                        {
                            List<string> logs = new List<string>();
                            logs.Add(serviceLogEntry); // Add the initial service log entry

                            // Keep consuming messages until serviceEndMatch is found
                            while (!serviceEndMatch.Success)
                            {
                                consumeResult2 = second_consumer.Consume(CancellationToken.None);
                                serviceLogEntry = consumeResult2.Message.Value;
                                logs.Add(serviceLogEntry);
                                serviceEndMatch = SharedConstants.ServiceEndRegex.Match(serviceLogEntry);
                            }

                            // Extract and parse service log data
                            AppLogEntity appLogEntity = ExtractServiceLog(logs);

                            // Insert into the database
                            InsertIntoDatabase(appLogEntity);

                            Console.WriteLine($"Inserted service log into database: {appLogEntity.ServiceCode}");
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Error consuming message: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public static AppLogEntity ExtractServiceLog(List<string> logs)
        {
            AppLogEntity appLogEntity = new AppLogEntity();
            bool requestDateTimeProcessed = false;
            bool responseDateTimeProcessed = false;

            foreach (string logEntry in logs)
            {
                Match threadIdMatch = SharedConstants.ThreadIdRegex.Match(logEntry);
                Match requestRegexMatch = SharedConstants.RequestRegex.Match(logEntry);
                Match requestDateTimeRegexMatch = SharedConstants.RequestDateTimeRegex.Match(logEntry);
                Match responseRegexMatch = SharedConstants.ResponseRegex.Match(logEntry);
                Match httpCodeRegexMatch = SharedConstants.HttpCodeRegex.Match(logEntry);

                Match responseDateTimeMatch = SharedConstants.ResponseDateTimeRegex.Match(logEntry);

                if (appLogEntity.ThreadId == null)
                    appLogEntity.ThreadId = threadIdMatch.Value;

                if (requestRegexMatch.Success && appLogEntity.ServiceCode == null)
                    appLogEntity.ServiceCode = requestRegexMatch.Groups[2].Value;

                if (requestDateTimeRegexMatch.Success && !requestDateTimeProcessed)
                {
                    string requestDateTimeString = requestDateTimeRegexMatch.Groups[2].Value;
                    int milliseconds = int.Parse(requestDateTimeRegexMatch.Groups[3].Value); // Extract milliseconds
                    DateTime requestDateTime = DateTime.ParseExact(requestDateTimeString, "d/M/yyyy H:m:s", CultureInfo.InvariantCulture);
                    appLogEntity.RequestDateTime = requestDateTime.AddMilliseconds(milliseconds); // Add milliseconds separately
                    requestDateTimeProcessed = true;
                }

                if (responseRegexMatch.Success && !responseDateTimeProcessed)
                {
                    Match responseDateTimeRegexMatch = SharedConstants.ResponseDateTimeRegex.Match(logEntry);
                    if (responseDateTimeRegexMatch.Success && !responseDateTimeProcessed)
                    {
                        string responseDateTimeString = responseDateTimeRegexMatch.Groups[0].Value;
                        appLogEntity.ResponseDateTime = DateTime.ParseExact(responseDateTimeString, "yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture);
                        responseDateTimeProcessed = true;
                        appLogEntity.ServiceTime = appLogEntity.ResponseDateTime - appLogEntity.RequestDateTime;
                    }
                }

                if (httpCodeRegexMatch.Success && appLogEntity.HttpCode == null)
                    appLogEntity.HttpCode = httpCodeRegexMatch.Groups[1].Value;
            }
            return appLogEntity;
        }

        public static void InsertIntoDatabase(AppLogEntity appLogEntity)
        {
            try
            {
                var procedureName = SharedConstants.SP_AddServiceLog;
                SqlParameter[] parameters =
                {
                        new SqlParameter("@ThreadId", appLogEntity.ThreadId),
                        new SqlParameter("@ServiceName", appLogEntity.ServiceCode),
                        new SqlParameter("@RequestDateTime", SqlDbType.DateTimeOffset) { Value = appLogEntity.RequestDateTime },
                        new SqlParameter("@ResponseDateTime", SqlDbType.DateTimeOffset) { Value = appLogEntity.ResponseDateTime },
                        new SqlParameter("@ServiceTime", appLogEntity.ServiceTime),
                        new SqlParameter("@HttpCode", int.Parse(appLogEntity.HttpCode))
                    };
                SqlDBHelper.ExecuteNonQuery(procedureName, CommandType.StoredProcedure, parameters);
                recordCounter++;
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error occurred: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
