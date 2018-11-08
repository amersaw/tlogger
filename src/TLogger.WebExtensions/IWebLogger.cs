using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLogger.WebExtensions
{
    public interface IWebLogger
    {
        void LogDiagnostics(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext httpContextAccessor = null);
        void LogException( string product, string layer, string activity, Exception ex, Dictionary<string, object> additionalInfo = null, HttpContext httpContextAccessor = null);
        void LogPerformance(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext httpContextAccessor = null);
        void LogUsage(string product, string layer, string activity, Dictionary<string, object> additionalInfo = null, HttpContext context = null);
    }
}
