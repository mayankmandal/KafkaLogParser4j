using Confluent.Kafka;
using KafkaLogConsumer.Utility;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KafkaLogEnricher
{
    public class KafkaLogEnricher
    {
        public static async Task Main(string[] args)
        {
            // Kafka configuration
            var kafkaBootstrapServers = "localhost:9092";
            var inputTopic = "transaction-logs-producer-25";
            var outputTopic = "transaction-logs-consumer-25";

            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaBootstrapServers,
                GroupId = "kafka-enricher-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            // Create a Kafka consumer
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(inputTopic);

            // Create a Kafka producer
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaBootstrapServers };
            using var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            string ServiceName = "";
            bool isInsideService = false;
            string ServiceThreadId = "";

            // Process incoming messages
            while (true)
            {
                try
                {
                    var consumeResult = consumer.Consume();
                    Match threadIdMatch = Constants.ThreadIdRegex.Match(consumeResult.Value);
                    Match serviceStartMatch = Constants.ServiceStartRegex.Match(consumeResult.Value);
                    Match serviceEndMatch = Constants.ServiceEndRegex.Match(consumeResult.Value);
                    if (isInsideService && !serviceEndMatch.Success)
                    {
                        await producer.ProduceAsync(outputTopic, new Message<Null, string> { Value = consumeResult.Value });
                        //Console.WriteLine($"Processed and published message to Kafka: {consumeResult.Value}");
                    }
                    else if (string.Equals(ServiceThreadId, threadIdMatch.Groups[1].Value) && serviceEndMatch.Success && string.Equals(ServiceName, serviceEndMatch.Groups[3].Value))
                    {
                        await producer.ProduceAsync(outputTopic, new Message<Null, string> { Value = consumeResult.Value });
                        //Console.WriteLine($"Processed and published message to Kafka: {consumeResult.Value}");
                        isInsideService = false;
                    }
                    else if (serviceStartMatch.Success)
                    {
                        isInsideService = true;
                        ServiceName = serviceStartMatch.Groups[1].Value;
                        await producer.ProduceAsync(outputTopic, new Message<Null, string> { Value = consumeResult.Value });
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
