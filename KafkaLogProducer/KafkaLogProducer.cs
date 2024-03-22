using Confluent.Kafka;
using KafkaClassLibrary;
using Microsoft.Data.SqlClient;
using System.Data;
namespace KafkaLogProducer
{
    public sealed class KafkaLogProducer
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        private readonly HashSet<string> processedFiles = new HashSet<string>(); // Maintain a collection of processed file paths
        private IProducer<Null, string> _producer = null;
        public KafkaLogProducer(IConfiguration configuration, ILogger<KafkaLogProducer> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        // Define a class to hold file path and status
        private class FileStatusInfo
        {
            public string FileName { get; set; }
            public string Status { get; set; }
        }
        public async Task ProducerMain(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting Kafka Servers...");
                await Task.Delay(TimeSpan.FromSeconds(1));

                // Access values from appsettings.json
                var logDirectoryPath = _configuration.GetSection("LogDirectoryPath").Value;
                var kafkaFirstProducerConfig = _configuration.GetSection("FirstProducerConfig");

                var firstproducerconfig = new ProducerConfig
                {
                    BootstrapServers = kafkaFirstProducerConfig["BootstrapServers"],
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.ScramSha512,
                    SaslUsername = kafkaFirstProducerConfig["SaslUsername"],
                    SaslPassword = kafkaFirstProducerConfig["SaslPassword"],
                    SslCaLocation = kafkaFirstProducerConfig["SslCaLocation"],
                    CompressionType = CompressionType.Gzip,
                    MessageSendMaxRetries = int.Parse(kafkaFirstProducerConfig["MessageSendMaxRetries"]),
                    Acks = Acks.All
                };

                // Create a Kafka producer
                _producer = new ProducerBuilder<Null, string>(firstproducerconfig).Build();

                // Check if the topic already exists
                await CheckTopicExists();

                _logger.LogInformation($"Initiate Producing Over Input Topic : {SharedVariables.InputTopic}");

                // Initialize the file watcher
                var fileWatcher = new FileSystemWatcher(logDirectoryPath);
                fileWatcher.EnableRaisingEvents = true;
                fileWatcher.Created += async (sender, e) => await ProcessNewLogFile(sender, e.FullPath);
                fileWatcher.Changed += async (sender, e) => await ProcessNewLogFile(sender, e.FullPath);

                // Check and Popoulate the FileProcessingStatus table for existing files
                await PopulateFileProcessingStatus(logDirectoryPath);

                // Process files with 'NS' or 'IP' status from database initially
                await ProcessFilesFromDatabase(logDirectoryPath);

                // Schedule file processing every 1 mintues
                await ScheduleFileProcessing(logDirectoryPath);

                _logger.LogInformation("Press any key to exit.");
            }
            catch (Exception ex)
            {
                _logger.LogError(exception: ex, "An error occurred in KafkaLogProducer: {Message}", ex.Message);
            }
        }
        private async Task CheckTopicExists()
        {
            // Check if the topic already exists
            try
            {
                var query = "SELECT [FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated] FROM [SpiderETMDB].[dbo].[TopicTrace]";
                DataTable dataTable = SqlDBHelper.ExecuteSelectCommand(query, CommandType.Text);
                if (dataTable.Rows.Count > 0)
                {
                    DataRow dataRow = dataTable.Rows[0];
                    SharedVariables.InputTopic = dataRow["FirstTopicName"] != DBNull.Value ? dataRow["FirstTopicName"].ToString() : SharedConstants.MagicString;
                    SharedVariables.OutputTopic = dataRow["SecondTopicName"] != DBNull.Value ? dataRow["SecondTopicName"].ToString() : SharedConstants.MagicString;
                    SharedVariables.IsInputTopicCreated = dataRow["isFirstTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isFirstTopicCreated"]) == 1 : false;
                    SharedVariables.IsOutputTopicCreated = dataRow["isSecondTopicCreated"] != DBNull.Value ? Convert.ToInt32(dataRow["isSecondTopicCreated"]) == 1 : false;
                    _logger.LogInformation($"Input Topic :'{SharedVariables.InputTopic}' data already exists over DB");
                }
                else
                {
                    query = "INSERT INTO [SpiderETMDB].[dbo].[TopicTrace] ([FirstTopicName], [SecondTopicName], [isFirstTopicCreated], [isSecondTopicCreated]) VALUES (@FirstTopicName, @SecondTopicName, @isFirstTopicCreated, @isSecondTopicCreated)";
                    SqlParameter[] sqlParameters = new SqlParameter[]
                    {
                        new SqlParameter("@FirstTopicName", SqlDbType.Text){Value = SharedVariables.InputTopic},
                        new SqlParameter("@SecondTopicName", SqlDbType.Text){Value = SharedVariables.OutputTopic},
                        new SqlParameter("@isFirstTopicCreated", SqlDbType.Int){Value = 1},
                        new SqlParameter("@isSecondTopicCreated", SqlDbType.Int){Value = 0},
                    };

                    SqlDBHelper.ExecuteNonQuery(query, CommandType.Text, sqlParameters);

                    _logger.LogInformation($"Input Topic :'{SharedVariables.InputTopic}' data inserted successfully into DB");

                    SharedVariables.IsInputTopicCreated = true;
                    SharedVariables.IsOutputTopicCreated = false;
                }
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"Observed Issue while using existing Input Topic: {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Observed Issue while using existing Input Topic: {ex.Message}");
            }
        }
        private async Task ProcessNewLogFile(Object sender, string filePath)
        {
            try
            {
                // Check if the file exists and hasn't been processed before
                if (File.Exists(filePath) && !processedFiles.Contains(filePath))
                {
                    var fileNameWithExtension = Path.GetFileName(filePath);

                    // Mark the file as processed 
                    processedFiles.Add(filePath);

                    // Check if the file already exists in the database
                    if (!(FileExistsInDatabase(fileNameWithExtension)))
                    {
                        InsertFileRecord(filePath, fileNameWithExtension);
                    }
                    // Process the newly added file
                    await ProcessFile(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing new log file '{filePath}': {ex.Message}");
            }
        }

        private async Task PopulateFileProcessingStatus(string logDirectoryPath)
        {
            try
            {
                // Check if the directory exists
                if (Directory.Exists(logDirectoryPath))
                {
                    // Enumerate files in the directory
                    foreach (var filePath in Directory.GetFiles(logDirectoryPath, "*", SearchOption.TopDirectoryOnly))
                    {
                        // Extract file name with extension
                        var fileNameWithExtension = Path.GetFileName(filePath);

                        // Check if the file already exists in the FileProcessingStatus table
                        if (!(FileExistsInDatabase(fileNameWithExtension)))
                        {
                            // If the file doesn't exist, insert a record into the FileProcessingStatus table
                            InsertFileRecord(filePath, fileNameWithExtension);
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Directory not found: {logDirectoryPath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error populating FileProcessingStatus table: {ex.Message}");
            }
        }

        private void InsertFileRecord(string filePath, string fileNameWithExtension)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.InsertRecord },
                    new SqlParameter("@FileNameWithExtension", SqlDbType.Text){Value = fileInfo.Name},
                    new SqlParameter("@Status", SqlDbType.Text){Value = "NS"},
                    new SqlParameter("@CurrentLineReadPosition", SqlDbType.Int){Value = 0}, // Initial position
                    new SqlParameter("@FileSize", SqlDbType.Int){Value = fileInfo.Length},
                };
                SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inserting file record for '{fileNameWithExtension}': {ex.Message}");
            }
        }

        private bool FileExistsInDatabase(string fileNameWithExtension)
        {
            try
            {
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] sqlParameter =
                {
                    new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.CheckExistence },
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100) {Value = fileNameWithExtension},
                };
                DataSet dataSet = SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, sqlParameter);
                if (dataSet.Tables != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];
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
                _logger.LogError($"Error fetching file record for '{fileNameWithExtension}': {ex.Message}");
            }
            return false;
        }

        private async Task ProcessFile(string filePath)
        {
            try
            {
                _logger.LogInformation($"Processing file: {filePath}");

                // Extract file name without extension
                var fileNameWithExtension = Path.GetFileName(filePath);

                // Get the current read position and total number of lines from the database
                var CurrentLineReadPosition = GetLastReadPosition(fileNameWithExtension);

                // Read file contents from the specified position onwards
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var streamReader = new StreamReader(fileStream))
                {
                    // Seek to the specified position in the file
                    fileStream.Seek(CurrentLineReadPosition, SeekOrigin.Begin);

                    // Read from the specified position to the end of the file
                    string remainingContent = streamReader.ReadToEnd();

                    // Produce each line to Kafka
                    var lines = remainingContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    var FileSize = CurrentLineReadPosition + remainingContent.Length;

                    foreach (var line in lines)
                    {
                        _producer.ProduceAsync(SharedVariables.InputTopic, new Message<Null, string> { Value = line });
                        CurrentLineReadPosition += line.Length;

                        // Update the last read position for the file
                        UpdateFileStatus(fileNameWithExtension, CurrentLineReadPosition, FileSize);
                        UpdateLastReadPosition(fileNameWithExtension, CurrentLineReadPosition, FileSize);
                    }
                }
                _logger.LogInformation($"Processed File: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing file '{filePath}': {ex.Message}");
            }
        }

        private long GetLastReadPosition(string fileNameWithExtension)
        {
            try
            {
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.GetCurrentLineReadPosition },
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100) {Value = fileNameWithExtension},
                };
                DataSet dataSet = SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
                if (dataSet.Tables != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];
                    DataRow dataRow = dataTable.Rows[0];
                    long CurrentLineReadPosition = Convert.ToInt64(dataRow["CurrentLineReadPosition"]);
                    return CurrentLineReadPosition;
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting file status for file '{fileNameWithExtension}': {ex.Message}");
                return 0;
            }
        }

        private void UpdateFileStatus(string fileNameWithExtension, long currentPosition, long FileSize)
        {
            try
            {
                // Determine the file status
                double progress = (double)currentPosition / FileSize;
                string status = progress >= 0.99 ? "CP" : "IP"; // In Progress or Completed

                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.UpdateStatusAndPosition },
                    new SqlParameter("@Status", SqlDbType.VarChar, 2){Value = status},
                    new SqlParameter("@CurrentLineReadPosition", SqlDbType.BigInt){Value = currentPosition},
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100){Value = fileNameWithExtension}
                };
                SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"Error updating file status for file '{fileNameWithExtension}': {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating file status for file '{fileNameWithExtension}': {ex.Message}");
            }
        }

        private string GetFileStatus(string filename)
        {
            try
            {
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] parameters =
                {
                     new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.GetStatus },
                     new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100) { Value = filename }
                };
                DataSet dataSet = SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
                if (dataSet.Tables != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];
                    DataRow dataRow = dataTable.Rows[0];
                    return dataRow["Status"].ToString();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting last read position for file '{filename}': {ex.Message}");
                return string.Empty;
            }
        }

        private void UpdateLastReadPosition(string filename, long lastReadPosition, long updatedFileSize)
        {
            try
            {
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] parameters =
                {
                    new SqlParameter("@State", SqlDbType.Int) { Value = (int)FileProcessingState.UpdatePositionAndFileSize },
                    new SqlParameter("@CurrentLineReadPosition", SqlDbType.BigInt){Value = lastReadPosition},
                    new SqlParameter("@UpdatedFileSize", SqlDbType.BigInt){Value = updatedFileSize},
                    new SqlParameter("@FileNameWithExtension", SqlDbType.VarChar, 100){Value = filename}
                };
                SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, parameters);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError($"Error updating last read position for file '{filename}': {sqlEx.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating last read position for file '{filename}': {ex.Message}");
            }
        }

        private List<FileStatusInfo> GetFilesToProcessFromDatabase(string logDirectoryPath)
        {
            List<FileStatusInfo> filesToProcess = new List<FileStatusInfo>();
            try
            {
                // Query the database to retrieve files with status NS or IP
                var procedureName = SharedConstants.SP_FileProcessingStatus;
                SqlParameter[] sqlParameters =
                {
                    new SqlParameter("@State",SqlDbType.Int){Value = (int)FileProcessingState.GetFilesToProcess}
                };
                DataSet dataSet = SqlDBHelper.ExecuteNonQueryWithResultSet(procedureName, CommandType.StoredProcedure, sqlParameters);
                if (dataSet.Tables != null && dataSet.Tables.Count > 0)
                {
                    DataTable dataTable = dataSet.Tables[0];
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string fileName = row["FileNameWithExtension"].ToString();
                        string status = row["Status"].ToString();
                        filesToProcess.Add(new FileStatusInfo { FileName = Path.Combine(logDirectoryPath, fileName), Status = status });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error retrieving files to process from the database: {ex.Message}");
            }
            return filesToProcess;
        }

        // Method to process files with 'NS' or 'IP' status from the database
        private async Task ProcessFilesFromDatabase(string logDirectoryPath)
        {
            List<FileStatusInfo> filesToProcess = (GetFilesToProcessFromDatabase(logDirectoryPath));
            foreach (var fileInfo in filesToProcess)
            {
                await ProcessFile(fileInfo.FileName);
            }
        }
        // Method to schedule file processing every 1 minutes
        private async Task ScheduleFileProcessing(string logDirectoryPath)
        {
            TimerCallback timerCallback = async (state) => await ProcessFilesFromDatabase(logDirectoryPath);
            Timer timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
    }
}
