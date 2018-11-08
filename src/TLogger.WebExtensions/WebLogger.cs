using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Serilog.Events;
using TLogger;

namespace TLogger.WebExtensions
{
    public class WebLogger : IWebLogger
    {
        private TLogger.Logger _logger;
        public WebLogger()
        {
            _logger = new TLogger.Logger();
        }

        public void LogUsage(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext context = null)
        {
            LogEntry entry = GetLogEntry(product, layer, activity, context, additionalInfo);
            if (context != null)
            {
                WriteUsage(entry);
            }

        }
        public void LogDiagnostics(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext context = null)
        {
            LogEntry entry = GetLogEntry(product, layer, activity, context, additionalInfo);
            if (context != null)
            {
                WritePerformance(entry);
            }

        }
        public void LogPerformance(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext context = null)
        {
            LogEntry entry = GetLogEntry(product, layer, activity, context, additionalInfo);
            if (context != null)
            {
                WritePerformance(entry);
            }
        }

        public void LogException(string product, string layer, string activity, Exception ex, Dictionary<string, object> additionalInfo = null, HttpContext context = null)
        {
            
            LogEntry entry = GetLogEntry(product, layer, activity, context, additionalInfo);
            entry.Exception = ex;
            if(context != null)
            {
                WriteError(entry);
            }
            

        }
        public  static LogEntry GetLogEntry(string product, string layer, string activityName,
            HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            //is that correct ?
            if (additionalInfo == null) additionalInfo = new Dictionary<string, object>();
            var entry = new LogEntry()
            {
                AdditionalInfo = additionalInfo,
                Product = product,
                Layer = layer,
                Message = activityName,
                Hostname = Environment.MachineName,
                CorrelationId = Activity.Current?.Id ?? context?.TraceIdentifier,
            };
            if (context != null)
            {
                GetRequestData(entry, context);
                GetUserData(entry, context.User);
            }
            //todo: Get Session Data
            //todo: Get Cookie Data
            return entry;
        }

        private static void GetUserData(LogEntry entry, ClaimsPrincipal user)
        {
            string userId = "", username = "";
            if (user != null)
            {
                int i = 1;
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                        userId = claim.Value;
                    if (claim.Type == "name")
                        username = claim.Value;

                    entry.AdditionalInfo.Add(string.Format("UserClaim-{0}-{1}", i++, claim.Type), claim.Value);
                }
            }
            entry.UserId = userId;
            entry.UserName = username;
        }

        private static void  GetRequestData(LogEntry entry, HttpContext context)
        {
            var request = context.Request;
            if (request != null)
            {
                entry.Location = request.Path;
                entry.AdditionalInfo.Add("UserAgent", request.Headers["User-Agent"]);
                entry.AdditionalInfo.Add("Languages", request.Headers["Accept-Language"]);
                var dict = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(request.QueryString.ToString());
                foreach (var key in dict.Keys)
                {
                    entry.AdditionalInfo.Add($"QueryString-{key}", dict[key].ToString());
                }
            }
        }



        internal void WritePerformance(LogEntry entry)
        {
            entry.EntryType = "Performance";
            _logger.Write(LogEventLevel.Information, "{@LogEntry}", entry);
        }
        internal void WriteUsage(LogEntry entry)
        {
            entry.EntryType = "Usage";
            _logger.Write(LogEventLevel.Information, "{@LogEntry}", entry);
        }
        internal void WriteError(LogEntry entry)
        {
            if (entry.Exception != null)
            {
                entry.EntryType = "Error";
                string procName = FindProcName(entry.Exception);
                entry.Location = string.IsNullOrEmpty(procName) ? entry.Location : procName;
                entry.Message = GetMessageFromException(entry.Exception);
                _logger.Write(LogEventLevel.Information, "{@LogEntry}", entry);

            }
        }
        internal void WriteDiagnostic(LogEntry entry)
        {
            entry.EntryType = "Diagnostic";
            var writeDiagnostics =
                Convert.ToBoolean(Environment.GetEnvironmentVariable("DIAGNOSTICS_ON"));
            if (writeDiagnostics)
                _logger.Write(LogEventLevel.Information, "{@LogEntry}", entry);

        }

        private string GetMessageFromException(Exception exception)
        {
            if (exception.InnerException != null)
                return GetMessageFromException(exception.InnerException);
            return exception.Message;
        }

        private string FindProcName(Exception exception)
        {
            return string.Empty;
        }
    }
}
