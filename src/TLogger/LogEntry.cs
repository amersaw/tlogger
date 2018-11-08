using System;
using System.Collections.Generic;

namespace TLogger
{
    public class LogEntry
    {
        public LogEntry()
        {
            Timestamp = DateTime.Now;
            //AdditionalInfo = new Dictionary<string, object>();
        }
        public string EntryType { get; set; }
        public DateTime Timestamp { get; set; }

        public string Message { get; set; }
        //Where
        public string Product { get; set; }
        public string Layer { get; set; }
        public string Location { get; set; }
        public string Hostname { get; set; }


        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }

        //Others
        public long? ElapsedMilliseconds { get; set; }
        public Exception Exception { get; set; }
        public string CorrelationId { get; set; }
        public Dictionary<string, object> AdditionalInfo { get; set; }

    }
}
