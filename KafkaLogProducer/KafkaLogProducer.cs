using Confluent.Kafka;
using Confluent.Kafka.Admin;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;

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
                    Console.WriteLine($"Observed Issue while using existing Input Topic: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Observed Issue while using existing Input Topic: {ex.Message}");
                }
            }
            else
            {
                // Create the topic 
                await CreateTopic(SharedConstants.kafkaBootstrapServers, kafkaTopic);

                try
                {
                    var query = "INSERT INTO [SpiderETMDB].[dbo].[TopicTrace] ([FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated]) VALUES (@FirstTopicName, @SecondTopicName, @isFirstTopicCreated, @isSecondTopicCreated)";
                    SqlParameter[] sqlParameters = new SqlParameter[] 
                    { 
                        new SqlParameter("@FirstTopicName", SqlDbType.Text){Value = kafkaTopic},
                        new SqlParameter("@SecondTopicName", DBNull.Value),
                        new SqlParameter("@isFirstTopicCreated", SqlDbType.Int){ Value = 1},
                        new SqlParameter("@isSecondTopicCreated", SqlDbType.Int){Value = 0},
                    };
                    
                    SqlDBHelper.ExecuteNonQuery(query, CommandType.Text, sqlParameters);
                    Console.WriteLine($"Topic '{kafkaTopic}' created successfully");

                    SharedVariables.InputTopic = kafkaTopic;
                    SharedVariables.OutputTopic = "";
                    SharedVariables.IsInputTopicCreated = true;
                    SharedVariables.IsOutputTopicCreated = false;
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine($"Observed Issue while Creating Input Topic: {sqlEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Observed Issue while Creating Input Topic: {ex.Message}");
                }
                
            }

            // Process files in the directory
            // await ProcessFilesInDirectory(SharedConstants.LogDirectoryPath, producer);

            // Initialize the file watcher
            var fileWatcher = new FileSystemWatcher(SharedConstants.LogDirectoryPath);
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Created += (sender, e) => ProcessFile(e.FullPath, producer);
            fileWatcher.Changed += (sender, e) => ProcessFile(e.FullPath, producer);

            // Check and Popoulate the FileProcessingStatus table for existing files
            await PopulateFileProcessingStatus();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

        static async Task PopulateFileProcessingStatus()
        {
            try
            {
                // Check if the directory exists
                if(Directory.Exists(SharedConstants.LogDirectoryPath))
                {
                    // Enumerate files in the directory
                    foreach(var filePath in Directory.GetFiles(SharedConstants.LogDirectoryPath))
                    {
                        // Check if the file already exists in the FileProcessingStatus table
                        if (!FileExistsInDatabase(filePath))
                        {
                            // If the file doesn't exist, insert a record into the FileProcessingStatus table
                            await InsertFileRecord(filePath);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Directory not found: {SharedConstants.LogDirectoryPath}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error populating FileProcessingStatus table: {ex.Message}");
            }
        }

        static async Task InsertFileRecord(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);

                // Insert a record into the FileProcessingStatus table
                var query = "INSERT INTO FileProcessingStatus (FilePath, Status, UpdateDate, CreateDate, PeekPosition, FileSize) " +
                                "VALUES (@FilePath, @Status, @UpdateDate, @CreateDate, @PeekPosition, @FileSize)";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FilePath", SqlDbType.Text){Value = fileInfo.Name},
                    new SqlParameter("@Status", SqlDbType.Text){Value = "NS"},
                    new SqlParameter("@UpdateDate", SqlDbType.DateTimeOffset) { Value = fileInfo.LastWriteTime },
                    new SqlParameter("@CreateDate", SqlDbType.DateTimeOffset) { Value = fileInfo.CreationTime },
                    new SqlParameter("@PeekPosition", SqlDbType.Int){Value = 0}, // Initial position
                    new SqlParameter("@FileSize", SqlDbType.Int){Value = fileInfo.Length},
                };
                SqlDBHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error inserting file record for '{filePath}': {ex.Message}");
            }
        }

        static bool FileExistsInDatabase(string filePath)
        {
            try
            {
                var query = "SELECT COUNT(*) FROM FileProcessingStatus WHERE FilePath = @FilePath";
                SqlParameter[] sqlParameter =
                {
                    new SqlParameter("@FilePath", filePath)
                };
                DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(query,CommandType.Text, sqlParameter);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    // Check if the DataTable contains a column named "Count"
                    if (dataTable.Columns.Contains("Count"))
                    {
                        // Retrieve the first row
                        var dataRow = dataTable.Rows[0];

                        // Check if the value in the "Count" column is not null and is convertible to int
                        if (dataRow["Count"] != DBNull.Value && int.TryParse(dataRow["Count"].ToString(), out int count))
                        {
                            return count > 0;
                        }
                    }
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching file record for '{filePath}': {ex.Message}");
            }
            return false;
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

        static async Task ProcessFile(string filePath, IProducer<Null, string> producer)
        {
            try
            {
                Console.WriteLine($"Processing file: {filePath}");

                // Read file contents
                var lines = await File.ReadAllLinesAsync(filePath);
                FileInfo fileInfo = new FileInfo(filePath);
                // Determine the last read position for this file
                var lastReadPosition = await GetLastReadPosition(fileInfo.Name);

                // Publish each unread line to Kafka starting from the last read position
                for(int i = lastReadPosition; i< lines.Length; i++)
                {
                    // Encode data as UTF-8
                    var line = lines[i];
                    var message = new Message<Null, string> { Value = line , Key = null};
                    await producer.ProduceAsync(SharedVariables.InputTopic, message);

                    // Update the last read position for this file
                    await UpdateLastReadPosition(fileInfo.Name, i);
                }
                // Flush messages to Kafka
                producer.Flush(TimeSpan.FromSeconds(10));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
            }
        }
        static async Task<int> GetLastReadPosition(string filename)
        {
            try
            {
                string query = "SELECT PeekPosition FROM FileProcessingStatus WHERE FilePath = @FilePath";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FilePath", SqlDbType.VarChar, 100) {Value = filename}
                };
                DataTable dataTable = await SqlDBHelper.ExecuteParameterizedSelectCommandAsync(query, CommandType.Text, parameters);
                if (dataTable != null)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    return Convert.ToInt32(dataRow["PeekPosition"]);
                }
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error getting last read position for file '{filename}': {ex.Message}");
                return 0;
            }
        }
        static async Task UpdateLastReadPosition(string filename, int lastReadPosition)
        {
            try
            {
                string query = "UPDATE FileProcessingStatus SET PeekPosition = @PeekPosition WHERE FilePath = @FilePath";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@PeekPosition", SqlDbType.BigInt){Value = lastReadPosition},
                    new SqlParameter("@FilePath", SqlDbType.VarChar, 100){Value = filename}
                };
                await SqlDBHelper.ExecuteNonQueryAsync(query, CommandType.Text, parameters);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error updating last read position for file '{filename}': {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating last read position for file '{filename}': {ex.Message}");
            }
        }
    }
}
