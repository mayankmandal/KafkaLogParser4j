using Confluent.Kafka;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;
using KafkaClassLibrary;

namespace KafkaLogConsumer
{
    public static class KafkaLogConsumer
    {
        private static int recordCounter = 0;

        public static async Task Main(string[] args)
        {
            do
            {
                Console.WriteLine("Waiting for Second Topic to be created...");
                await Task.Delay(5000);
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

            var config = new ConsumerConfig
            {
                BootstrapServers = SharedConstants.kafkaBootstrapServers,
                GroupId = "consumer-group-1",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // SQL Server configuration
            string connectionString = SharedConstants.DBConnectionString;

            // Create Kafka consumer
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

            // Subscribe to Kafka topic
            consumer.Subscribe(SharedVariables.OutputTopic);

            // Start consuming messages
            Console.WriteLine("Consuming messages from Kafka...");
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);

                    if (consumeResult != null)
                    {
                        string serviceLogEntry = consumeResult.Message.Value;
                        Match serviceStartMatch = SharedConstants.ServiceStartRegex.Match(serviceLogEntry);
                        Match serviceEndMatch = SharedConstants.ServiceEndRegex.Match(serviceLogEntry);

                        if (serviceStartMatch.Success)
                        {
                            List<string> logs = new List<string>();
                            logs.Add(serviceLogEntry); // Add the initial service log entry

                            // Keep consuming messages until serviceEndMatch is found
                            while (!serviceEndMatch.Success)
                            {
                                consumeResult = consumer.Consume(CancellationToken.None);
                                serviceLogEntry = consumeResult.Message.Value;
                                logs.Add(serviceLogEntry);
                                serviceEndMatch = SharedConstants.ServiceEndRegex.Match(serviceLogEntry);
                            }

                            // Extract and parse service log data
                            AppLogEntity appLogEntity = ExtractServiceLog(logs);

                            // Insert into the database
                            InsertIntoDatabase(appLogEntity, connectionString);

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

        public static void InsertIntoDatabase(AppLogEntity appLogEntity, string connectionString)
        {
            // Create a connection to the SQL Server Database
            using (SqlConnection connection = new SqlConnection(connectionString))
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
}
