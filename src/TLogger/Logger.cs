using Amazon.CloudWatchLogs;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLogger
{
    public class Logger
    {
        private readonly Serilog.ILogger _logger;
        private readonly string name;

        //public Logger(TLoggerConfig cfg): this(cfg.Type,cfg.Name)
        //{
        //}
        public Logger(string type = "file", string name = "_")
        {
            if (type == "file")
            {
                _logger = new LoggerConfiguration()
                    .WriteTo.File(path: $"logs\\{name}")
                    .CreateLogger();
            }
            else if (type == "tcp")
            {
                _logger = new LoggerConfiguration()
                    .WriteTo.TcpSyslog("logs#.papertrailapp.com:####")
                    .CreateLogger();

            }
            else if (type == "aws")
            {
                var client = new AmazonCloudWatchLogsClient(Amazon.RegionEndpoint.EUCentral1);
                ICloudWatchSinkOptions awsConfig;

                {
                    // name of the log group
                    var logGroupName = $"/aws/{name}";


                    // options for the sink defaults in https://github.com/Cimpress-MCP/serilog-sinks-awscloudwatch/blob/master/src/Serilog.Sinks.AwsCloudWatch/CloudWatchSinkOptions.cs
                    awsConfig = new CloudWatchSinkOptions
                    {
                        // the name of the CloudWatch Log group for logging
                        LogGroupName = logGroupName,

                        //// the main formatter of the log event
                        TextFormatter = new CompactJsonFormatter(),

                        // other defaults defaults
                        MinimumLogEventLevel = LogEventLevel.Information,
                        BatchSizeLimit = 100,
                        QueueSizeLimit = 10000,
                        Period = TimeSpan.FromSeconds(10),
                        CreateLogGroup = true,
                        LogStreamNameProvider = new DefaultLogStreamProvider(),
                        RetryAttempts = 5
                    };
                    //return options;
                }
                _logger = new LoggerConfiguration()
                    .WriteTo.AmazonCloudWatch(awsConfig, client)
                    .CreateLogger();
                //throw new NotImplementedException();
            }

            this.name = name;
        }

        public void Write(LogEventLevel information, string v, LogEntry entry)
        {
            _logger.Write(information, v, entry);
        }
    }
}
