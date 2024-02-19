using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;

namespace KafkaLogProducer
{
    public class KafkaLogProducer
    {
        public static async Task Main(string[] args)
        {
            var config = new ProducerConfig { BootstrapServers = SharedConstants.kafkaBootstrapServers };

            // Create a Kafka producer
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            // Prompt the user for the topic name
            Console.WriteLine("Enter the Topic Name: ");
            var kafkaTopic = Console.ReadLine();

            // Check if the topic already exists
            if (await TopicExists(SharedConstants.kafkaBootstrapServers, kafkaTopic))
            {
                Console.WriteLine($"Topic {kafkaTopic} already exists");
                using (var connection = new SqlConnection(SharedConstants.DBConnectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT [FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated] FROM [SpiderETMDB].[dbo].[TopicTrace]";
                    using (var command = new SqlCommand(query,connection))
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
            else
            {
                // Create the topic 
                await CreateTopic(SharedConstants.kafkaBootstrapServers, kafkaTopic);

                using (var connection = new SqlConnection(SharedConstants.DBConnectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO [SpiderETMDB].[dbo].[TopicTrace] ([FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated]) VALUES (@FirstTopicName, NULL, 1, 0)";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstTopicName", kafkaTopic);
                        command.ExecuteNonQuery();
                    }
                }
                Console.WriteLine($"Topic '{kafkaTopic}' created successfully");
                SharedVariables.InputTopic = kafkaTopic;
                SharedVariables.OutputTopic = "";
                SharedVariables.IsInputTopicCreated = true;
                SharedVariables.IsOutputTopicCreated = false;
            }

            // Directory containing log files
            var logDirectoryPath = @"C:\Users\MayankCoinStation\Downloads\SiteSelectionLog";


            // Process files in the directory
            await ProcessFilesInDirectory(logDirectoryPath, producer, kafkaTopic);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static async Task<bool> TopicExists(string bootstrapServers, string topicName)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            return metadata.Topics.Any(t => t.Topic == topicName);
        }

        static async Task CreateTopic(string bootstrapServers, string topicName)
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = bootstrapServers }).Build();
            await adminClient.CreateTopicsAsync(new TopicSpecification[] { new TopicSpecification { Name = topicName, NumPartitions = 1, ReplicationFactor = 1 } });
        }

        static async Task ProcessFilesInDirectory(string directoryPath, IProducer<Null, string> producer, string kafkaTopic)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(directoryPath))
                {
                    // Enumerate files in the directory
                    foreach (var filePath in Directory.GetFiles(directoryPath))
                    {
                        await ProcessFile(filePath, producer, kafkaTopic);
                        Console.WriteLine($"File: {filePath} is streamed on Topic: {SharedVariables.InputTopic}");
                    }
                }
                else
                {
                    Console.WriteLine($"Directory not found: {directoryPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing files: {ex.Message}");
            }
        }

        static async Task ProcessFile(string filePath, IProducer<Null, string> producer, string kafkaTopic)
        {
            try
            {
                Console.WriteLine($"Processing file: {filePath}");

                // Read file contents
                var lines = await File.ReadAllLinesAsync(filePath);

                // Publish each line to Kafka
                foreach (var line in lines)
                {
                    // Encode data as UTF-8
                    var message = new Message<Null, string> { Value = line, Key = null };
                    await producer.ProduceAsync(kafkaTopic, message);
                    //Console.WriteLine($"Published to Kafka: {line}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
            }
        }
    }
}
