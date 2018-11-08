using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using TLogger;
namespace TLogger.WebExtensions
{
    public class PerformanceTracker
    {
        private readonly Stopwatch _sw;
        private readonly LogEntry _entry;
        private readonly WebLogger _logger;

        public PerformanceTracker(LogEntry entry, WebLogger logger)
        {
            _sw = Stopwatch.StartNew();
            _entry = entry;
            _logger = logger;
            DateTime beginTime = DateTime.Now;
            if (_entry.AdditionalInfo == null)
                _entry.AdditionalInfo = new Dictionary<string, object>()
                {
                    { "Started", beginTime.ToString(CultureInfo.InvariantCulture) }
                };
            else
                _entry.AdditionalInfo.Add(
                    "Started", beginTime.ToString(CultureInfo.InvariantCulture));

        }
        public PerformanceTracker(string name, string userId, string userName,
          string location, string product, string layer)
        {
            _entry = new LogEntry()
            {
                Message = name,
                UserId = userId,
                UserName = userName,
                Product = product,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName
            };

            var beginTime = DateTime.Now;
            _entry.AdditionalInfo = new Dictionary<string, object>()
            {
                { "Started", beginTime.ToString(CultureInfo.InvariantCulture)  }
            };
        }

        public PerformanceTracker(string name, string userId, string userName,
                   string location, string product, string layer,
                   Dictionary<string, object> perfParams)
            : this(name, userId, userName, location, product, layer)
        {
            foreach (var item in perfParams)
                _entry.AdditionalInfo.Add("input-" + item.Key, item.Value);
        }

        public void Stop()
        {
            _sw.Stop();
            _entry.ElapsedMilliseconds = _sw.ElapsedMilliseconds;
            _logger.WritePerformance(_entry);
        }
    }
}
