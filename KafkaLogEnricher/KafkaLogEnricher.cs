using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;

namespace KafkaLogEnricher
{
    public class KafkaLogEnricher
    {
        public static async Task Main(string[] args)
        {
            do
            {
                Console.WriteLine("Waiting for First Topic to be created...");
                await Task.Delay(5000);

                using (var connection = new SqlConnection(SharedConstants.DBConnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT [FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated] FROM [SpiderETMDB].[dbo].[TopicTrace]";
                    using (var command = new SqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.HasRows)
                            {
                                await reader.ReadAsync();
                                SharedVariables.InputTopic = reader["FirstTopicName"] != DBNull.Value ? reader["FirstTopicName"].ToString() : "";
                                SharedVariables.OutputTopic = reader["SecondTopicName"] != DBNull.Value ? reader["SecondTopicName"].ToString() : "";
                                SharedVariables.IsInputTopicCreated = reader["isFirstTopicCreated"] != DBNull.Value ? Convert.ToInt32(reader["isFirstTopicCreated"]) == 1 : false;
                                SharedVariables.IsOutputTopicCreated = reader["isSecondTopicCreated"] != DBNull.Value ? Convert.ToInt32(reader["isSecondTopicCreated"]) == 1 : false;
                            }
                        }
                    }
                }
            }
            while (!SharedVariables.IsInputTopicCreated); // Wait for the first topic to be created

            // Kafka configuration
            SharedVariables.OutputTopic = SharedVariables.InputTopic + "-second";

            var config = new ConsumerConfig
            {
                BootstrapServers = SharedConstants.kafkaBootstrapServers,
                GroupId = "kafka-enricher-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // Create a Kafka consumer
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(SharedVariables.InputTopic);

            // Create a Kafka producer
            var producerConfig = new ProducerConfig { BootstrapServers = SharedConstants.kafkaBootstrapServers };
            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            string ServiceName = "";
            bool isInsideService = false;
            string ServiceThreadId = "";
            SharedVariables.IsOutputTopicCreated = true;

            using (var connection = new SqlConnection(SharedConstants.DBConnectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE [SpiderETMDB].[dbo].[TopicTrace] SET [FirstTopicName] = @FirstTopicName, [SecondTopicName] = @SecondTopicName, [isFirstTopicCreated] = @isFirstTopicCreated, [isSecondTopicCreated] = @isSecondTopicCreated";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FirstTopicName", SharedVariables.InputTopic);
                    command.Parameters.AddWithValue("@SecondTopicName", SharedVariables.OutputTopic);
                    command.Parameters.AddWithValue("@isFirstTopicCreated", SharedVariables.IsInputTopicCreated == true? 1:0);
                    command.Parameters.AddWithValue("@isSecondTopicCreated", SharedVariables.IsOutputTopicCreated == true ? 1 : 0);
                    await command.ExecuteNonQueryAsync();
                }
            }
            Console.WriteLine($"Second Topic created: {SharedVariables.OutputTopic}");

            // Process incoming messages
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume();
                    Match threadIdMatch = SharedConstants.ThreadIdRegex.Match(consumeResult.Value);
                    Match serviceStartMatch = SharedConstants.ServiceStartRegex.Match(consumeResult.Value);
                    Match serviceEndMatch = SharedConstants.ServiceEndRegex.Match(consumeResult.Value);
                    if (isInsideService && !serviceEndMatch.Success)
                    {
                        await producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult.Value });
                    }
                    else if (string.Equals(ServiceThreadId, threadIdMatch.Groups[1].Value) && serviceEndMatch.Success && string.Equals(ServiceName, serviceEndMatch.Groups[3].Value))
                    {
                        await producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult.Value });
                        Console.WriteLine($"Published Service: {ServiceName} to Kafka");
                        isInsideService = false;
                    }
                    else if (serviceStartMatch.Success)
                    {
                        isInsideService = true;
                        ServiceName = serviceStartMatch.Groups[1].Value;
                        await producer.ProduceAsync(SharedVariables.OutputTopic, new Message<Null, string> { Value = consumeResult.Value });
                        //Console.WriteLine($"Processed and published message to Kafka: {consumeResult.Value}");
                        if (threadIdMatch.Success)
                        {
                            ServiceThreadId = threadIdMatch.Groups[1].Value;
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occurred: {e.Error.Reason}");
                }
            }
        }
    }
}
