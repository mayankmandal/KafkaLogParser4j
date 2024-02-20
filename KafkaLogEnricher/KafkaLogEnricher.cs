using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;

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
                try
                {
                    var query = "SELECT [FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated] FROM [SpiderETMDB].[dbo].[TopicTrace]";
                    DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(query, CommandType.Text);
                    if(dataTable != null )
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
                SqlDBHelper.ExecuteNonQuery(query, CommandType.Text,parameters);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error updating last read position for file : {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating last read position for file : {ex.Message}");
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
