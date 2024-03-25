using Microsoft.Identity.Client;
using System.Text.RegularExpressions;

namespace KafkaClassLibrary
{
    public static class SharedConstants
    {

        public const string SP_AddServiceLog = "[dbo].[uspAddServiceLog]";
        public const string SP_FileProcessingStatus = "[dbo].[uspFileProcessingStatus]";
        public const string SP_TopicTrace = "[dbo].[uspTopicTrace]";

        // Define regular expressions
        public static readonly Regex ThreadIdRegex = new Regex(@"\[(.*?)\]");
        public static readonly Regex ServiceStartRegex = new Regex(@"MessageSource\s*::\s*Tran Code\s*\[(\w+)\]\s*::\s*Request Function\s*\[(\w+)\]");
        public static readonly Regex RequestRegex = new Regex(@"Transaction\s*::\s*([^\s]+)\s*::\s*Request\s*\[([^\]]+)\]");
        public static readonly Regex RequestDateTimeRegex = new Regex(@"Request DateTime \[CONVERT\(datetime,('([\w\s:/]+)'),(\d+)\)\]");
        public static readonly Regex ResponseRegex = new Regex(@"Transaction\s*::\s*([^\s]+)\s*::\s*Response\s*\[([^\]]+)\]");
        public static readonly Regex ResponseDateTimeRegex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}),\d{3}");
        public static readonly Regex HttpCodeRegex = new Regex(@"<HttpCode>(\d+)<\/HttpCode>");
        public static readonly Regex ServiceEndRegex = new Regex(@"Transaction ::\s+([\w\s]+) ::\s+([\w\s]+)\s+\[(\w+)\] \[\] --- End ----");

        public static readonly string MagicString = string.Empty;
    }
    public enum FileProcessingState
    {
        CheckExistence = 1,
        GetCurrentLineReadPosition = 2,
        GetStatus = 3,
        GetFilesToProcess = 4,
        InsertRecord = 5,
        UpdateStatusAndPosition = 6,
        UpdatePositionAndFileSize = 7,
        UpdatePositionOnly = 8,
        GetFileCurrentState = 9,
        DeleteFileRow = 10
    }
    public enum TopicState
    {
        CheckExistence = 1,
        InsertData = 2,
        UpdateData = 3,
        DeleteData = 4
    }

    public static class FileStatus
    {
        public static string Completed = "CP";
        public static string InProgress = "IP";
        public static string NotStarted = "NS";
    }

}
