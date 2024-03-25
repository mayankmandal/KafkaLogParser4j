
namespace KafkaClassLibrary
{
    public class AppLogEntity
    {
        public string ThreadId { get; set; }
        public string ServiceCode { get; set; }
        public DateTime RequestDateTime { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public TimeSpan ServiceTime { get; set; }
        public string HttpCode { get; set; }
    }
    public class FileProcessingStatusEntity
    {
        public long Id { get; set; }
        public string FileNameWithExtension { get; set; }
        public string Status { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreateDate { get; set; }
        public long? CurrentLineReadPosition { get; set; }
        public long? FileSize { get; set; }
    }
}
