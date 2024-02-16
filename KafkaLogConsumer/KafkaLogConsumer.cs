using Confluent.Kafka;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading;
using static Confluent.Kafka.ConfigPropertyNames;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Abstractions;
using System.Globalization;
using KafkaLogConsumer.Utility;

    namespace KafkaLogConsumer
    {
        public static class KafkaLogConsumer
        {
            private static int recordCounter = 0;

            public static async Task Main(string[] args)
            {
                // Kafka configuration
                var kafkaBootstrapServers = "localhost:9092";
                var kafkaTopic = "transaction-logs-consumer-25";

                var config = new ConsumerConfig
                {
                    BootstrapServers = kafkaBootstrapServers,
                    GroupId = "consumer-group-1",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                // SQL Server configuration
                string connectionString = Constants.DBConnectionString;

                // Create Kafka consumer
                using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();

                // Subscribe to Kafka topic
                consumer.Subscribe(kafkaTopic);

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
                            Match serviceStartMatch = Constants.ServiceStartRegex.Match(serviceLogEntry);
                            Match serviceEndMatch = Constants.ServiceEndRegex.Match(serviceLogEntry);

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
                                    serviceEndMatch = Constants.ServiceEndRegex.Match(serviceLogEntry);
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
                    Match threadIdMatch = Constants.ThreadIdRegex.Match(logEntry);
                    Match requestRegexMatch = Constants.RequestRegex.Match(logEntry);
                    Match requestDateTimeRegexMatch = Constants.RequestDateTimeRegex.Match(logEntry);
                    Match responseRegexMatch = Constants.ResponseRegex.Match(logEntry);
                    Match httpCodeRegexMatch = Constants.HttpCodeRegex.Match(logEntry);

                    Match responseDateTimeMatch = Constants.ResponseDateTimeRegex.Match(logEntry);

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
                        Match responseDateTimeRegexMatch = Constants.ResponseDateTimeRegex.Match(logEntry);
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
                        connection.Open();
                        // Insert into the database
                        using (SqlCommand insertCommand = new SqlCommand(Constants.SP_AddServiceLog, connection))
                        {
                            insertCommand.CommandType = CommandType.StoredProcedure;

                            insertCommand.Parameters.AddWithValue("@ThreadId", appLogEntity.ThreadId);
                            insertCommand.Parameters.AddWithValue("@ServiceName", appLogEntity.ServiceCode);
                            insertCommand.Parameters.Add("@RequestDateTime", SqlDbType.DateTimeOffset).Value = appLogEntity.RequestDateTime;
                            insertCommand.Parameters.Add("@ResponseDateTime", SqlDbType.DateTimeOffset).Value = appLogEntity.ResponseDateTime;
                            insertCommand.Parameters.AddWithValue("@ServiceTime", appLogEntity.ServiceTime);
                            insertCommand.Parameters.AddWithValue("@HttpCode", int.Parse(appLogEntity.HttpCode));
                            insertCommand.ExecuteNonQuery();
                        };
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
