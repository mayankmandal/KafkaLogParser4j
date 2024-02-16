using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KafkaLogProducer
{
    public class KafkaLogProducer
    {
        public static async Task Main(string[] args)
        {
            // Kafka configuration
            var kafkaBootstrapServers = "localhost:9092";
            var kafkaTopic = "transaction-logs-producer-25";

            // Directory containing log files
            var logDirectoryPath = @"C:\Users\MayankCoinStation\Downloads\SiteSelectionLog";

            var config = new ProducerConfig { BootstrapServers = kafkaBootstrapServers };

            // Create a Kafka producer
            using var producer = new ProducerBuilder<Null, string>(config).Build();

            // Process files in the directory
            await ProcessFilesInDirectory(logDirectoryPath, producer, kafkaTopic);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
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
