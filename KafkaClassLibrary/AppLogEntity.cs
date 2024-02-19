using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
