using System.Text.RegularExpressions;

namespace KafkaClassLibrary
{
    public static class SharedConstants
    {

        public const string SP_AddServiceLog = "dbo.uspAddServiceLog";

        // Define regular expressions
        public static readonly Regex ThreadIdRegex = new Regex(@"\[(https-jsse-nio-8443-exec-(\d+))\]");
        public static readonly Regex ServiceStartRegex = new Regex(@"MessageSource\s*::\s*Tran Code\s*\[(\w+)\]\s*::\s*Request Function\s*\[(\w+)\]");
        public static readonly Regex RequestRegex = new Regex(@"Transaction\s*::\s*([^\s]+)\s*::\s*Request\s*\[([^\]]+)\]");
        public static readonly Regex RequestDateTimeRegex = new Regex(@"Request DateTime \[CONVERT\(datetime,('([\w\s:/]+)'),(\d+)\)\]");
        public static readonly Regex ResponseRegex = new Regex(@"Transaction\s*::\s*([^\s]+)\s*::\s*Response\s*\[([^\]]+)\]");
        public static readonly Regex ResponseDateTimeRegex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}),\d{3}");
        public static readonly Regex HttpCodeRegex = new Regex(@"<HttpCode>(\d+)<\/HttpCode>");
        public static readonly Regex ServiceEndRegex = new Regex(@"Transaction ::\s+([\w\s]+) ::\s+([\w\s]+)\s+\[(\w+)\] \[\] --- End ----");
    }
}
