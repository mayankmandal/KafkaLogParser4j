using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;

namespace KafkaLogProducer
{
    public class KafkaLogProducer
    {
        // Define a class to hold file path and status
        private class FileStatusInfo
        {
            public string FileName { get; set; }
            public string Status { get; set; }
        }
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

            // Initialize the file watcher
            var fileWatcher = new FileSystemWatcher(SharedConstants.LogDirectoryPath);
            fileWatcher.EnableRaisingEvents = true;
            fileWatcher.Created += (sender, e) => ProcessFile(e.FullPath, producer);
            fileWatcher.Changed += (sender, e) => ProcessFile(e.FullPath, producer);

            // Check and Popoulate the FileProcessingStatus table for existing files
            await PopulateFileProcessingStatus(producer);

            // Process files with 'NS' or 'IP' status from database initially
            await ProcessFilesFromDatabase(producer);

            // Schedule file processing every 15 mintues
            ScheduleFileProcessing(producer);

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

        static async Task PopulateFileProcessingStatus(IProducer<Null, string> producer)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(SharedConstants.LogDirectoryPath))
                {
                    // Enumerate files in the directory
                    foreach (var filePath in Directory.GetFiles(SharedConstants.LogDirectoryPath, "*", SearchOption.TopDirectoryOnly))
                    {
                        // Extract file name with extension
                        var fileNameWithExtension = Path.GetFileName(filePath);

                        // Check if the file already exists in the FileProcessingStatus table
                        if (!FileExistsInDatabase(fileNameWithExtension))
                        {
                            // If the file doesn't exist, insert a record into the FileProcessingStatus table
                            await InsertFileRecord(filePath, fileNameWithExtension);
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Directory not found: {SharedConstants.LogDirectoryPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error populating FileProcessingStatus table: {ex.Message}");
            }
        }

        static async Task InsertFileRecord(string filePath, string fileNameWithExtension)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);

                // Insert a record into the FileProcessingStatus table
                var query = "INSERT INTO FileProcessingStatus (FileNameWithExtension, Status, UpdateDate, CreateDate, CurrentLineReadPosition, FileSize) " +
                                "VALUES (@FileNameWithExtension, @Status, @UpdateDate, @CreateDate, @CurrentLineReadPosition, @FileSize)";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FileNameWithExtension", SqlDbType.Text){Value = fileInfo.Name},
                    new SqlParameter("@Status", SqlDbType.Text){Value = "NS"},
                    new SqlParameter("@UpdateDate", SqlDbType.DateTimeOffset) { Value = fileInfo.LastWriteTime },
                    new SqlParameter("@CreateDate", SqlDbType.DateTimeOffset) { Value = fileInfo.CreationTime },
                    new SqlParameter("@CurrentLineReadPosition", SqlDbType.Int){Value = 0}, // Initial position
                    new SqlParameter("@FileSize", SqlDbType.Int){Value = fileInfo.Length},
                };
                SqlDBHelper.ExecuteNonQuery(query, CommandType.Text, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting file record for '{fileNameWithExtension}': {ex.Message}");
            }
        }

        static bool FileExistsInDatabase(string fileNameWithExtension)
        {
            try
            {
                var query = "SELECT COUNT(*) AS TOTAL FROM FileProcessingStatus WHERE FileNameWithExtension = @FileNameWithExtension";
                SqlParameter[] sqlParameter =
                {
                    new SqlParameter("@FileNameWithExtension", fileNameWithExtension)
                };
                DataTable dataTable = SqlDBHelper.ExecuteParameterizedSelectCommand(query, CommandType.Text, sqlParameter);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    // Check if the DataTable contains a column named "Count"
                    if (dataTable.Columns.Contains("TOTAL"))
                    {
                        // Retrieve the first row
                        var dataRow = dataTable.Rows[0];

                        // Check if the value in the "Count" column is not null and is convertible to int
                        if (dataRow["TOTAL"] != DBNull.Value && int.TryParse(dataRow["TOTAL"].ToString(), out int count))
                        {
                            return count > 0;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching file record for '{fileNameWithExtension}': {ex.Message}");
            }
            return false;
        }

        static async Task ProcessFile(string filePath, IProducer<Null, string> producer)
        {
            try
            {
                Console.WriteLine($"Processing file: {filePath}");

                // Extract file name without extension
                var fileNameWithExtension = Path.GetFileName(filePath);

                // Get the current read position and total number of lines from the database
                var (CurrentLineReadPosition, FileSize) = await GetLastReadPosition(fileNameWithExtension);
                // If the current read position is less than the total number of lines, continue processing
                if (CurrentLineReadPosition < FileSize)
                {
                    // Read file contents
                    var lines = await File.ReadAllLinesAsync(filePath);
                    int rowCount = 0;
                    long currentPosition = CurrentLineReadPosition;
                    // Publish each unread line to Kafka starting from the last read position
                    for (; currentPosition < FileSize && rowCount < lines.Length; rowCount++)
                    {
                        // Encode data as UTF-8
                        var line = lines[rowCount];
                        var message = new Message<Null, string> { Value = line, Key = null };
                        await producer.ProduceAsync(SharedVariables.InputTopic, message);
                        currentPosition += line.Length;

                        // Update the last read position for this file
                        await UpdateFileStatus(fileNameWithExtension, currentPosition, FileSize);
                        await UpdateLastReadPosition(fileNameWithExtension, (int)currentPosition - 1);
                    }

                    /* // Update the last read position for this file
                    await UpdateFileStatus(fileNameWithExtension, currentPosition, FileSize);
                    await UpdateLastReadPosition(fileNameWithExtension, (int)currentPosition - 1);*/
                }
                Console.WriteLine($"Processed File: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file '{filePath}': {ex.Message}");
            }
        }

        static async Task<(long, long)> GetLastReadPosition(string fileNameWithExtension)
        {
            try
            {
                string query = "SELECT CurrentLineReadPosition, FileSize FROM FileProcessingStatus WHERE FileNameWithExtension = @FileNameWithExtension";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100) {Value = fileNameWithExtension}
                };
                DataTable dataTable = await SqlDBHelper.ExecuteParameterizedSelectCommandAsync(query, CommandType.Text, parameters);
                if (dataTable != null && dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    long CurrentLineReadPosition = Convert.ToInt64(dataRow["CurrentLineReadPosition"]);
                    long FileSize = Convert.ToInt64(dataRow["FileSize"]);
                    return (CurrentLineReadPosition, FileSize);
                }
                return (0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting file status for file '{fileNameWithExtension}': {ex.Message}");
                return (0, 0);
            }
        }

        static async Task UpdateFileStatus(string fileNameWithExtension, long currentPosition, long FileSize)
        {
            try
            {
                // Determine the file status
                string status = currentPosition < FileSize ? "IP" : "CP"; // In Progress or Completed

                string query = "UPDATE FileProcessingStatus SET Status = @Status, CurrentLineReadPosition = @CurrentPosition WHERE FileNameWithExtension = @FileNameWithExtension";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Status", SqlDbType.VarChar, 2){Value = status},
                    new SqlParameter("@CurrentPosition", SqlDbType.BigInt){Value = currentPosition},
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100){Value = fileNameWithExtension}
                };
                await SqlDBHelper.ExecuteNonQueryAsync(query, CommandType.Text, parameters);
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Error updating file status for file '{fileNameWithExtension}': {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating file status for file '{fileNameWithExtension}': {ex.Message}");
            }
        }

        static async Task<string> GetFileStatus(string filename)
        {
            try
            {
                string query = "SELECT [Status] FROM FileProcessingStatus WHERE FileNameWithExtension = @FileNameWithExtension";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100) {Value = filename}
                };
                DataTable dataTable = await SqlDBHelper.ExecuteParameterizedSelectCommandAsync(query, CommandType.Text, parameters);
                if (dataTable != null)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    return dataRow["Status"].ToString();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting last read position for file '{filename}': {ex.Message}");
                return string.Empty;
            }
        }

        static async Task UpdateLastReadPosition(string filename, int lastReadPosition)
        {
            try
            {
                string query = "UPDATE FileProcessingStatus SET CurrentLineReadPosition = @CurrentLineReadPosition WHERE FileNameWithExtension = @FileNameWithExtension";
                SqlParameter[] parameters =
                {
                    new SqlParameter("@CurrentLineReadPosition", SqlDbType.BigInt){Value = lastReadPosition},
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100){Value = filename}
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

        private static List<FileStatusInfo> GetFilesToProcessFromDatabase()
        {
            List<FileStatusInfo> filesToProcess = new List<FileStatusInfo>();
            try
            {
                // Query the database to retrieve files with status NS or IP
                string query = "SELECT FileNameWithExtension, Status FROM FileProcessingStatus WHERE Status IN ('NS', 'IP')";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(query, CommandType.Text);
                if (dataTable != null)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string fileName = row["FileNameWithExtension"].ToString();
                        string status = row["Status"].ToString();
                        filesToProcess.Add(new FileStatusInfo { FileName = Path.Combine(SharedConstants.LogDirectoryPath, fileName), Status = status });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving files to process from the database: {ex.Message}");
            }
            return filesToProcess;
        }

        // Method to process files with 'NS' or 'IP' status from the database
        private static async Task ProcessFilesFromDatabase(IProducer<Null, string> producer)
        {
            List<FileStatusInfo> filesToProcess = GetFilesToProcessFromDatabase();
            foreach(var fileInfo in filesToProcess)
            {
                await ProcessFile(fileInfo.FileName, producer);
            }
        }
        // Method to schedule file processing every 15 minutes
        private static void ScheduleFileProcessing(IProducer<Null, string> producer)
        {
            TimerCallback timerCallback = async (state) => await ProcessFilesFromDatabase(producer);
            Timer timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
    }
}
