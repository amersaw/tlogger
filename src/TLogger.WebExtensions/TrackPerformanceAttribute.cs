using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLogger.WebExtensions
{
    public class TrackPerformanceAttribute : ActionFilterAttribute
    {
        private readonly string _product;
        private readonly string _layer;
        private readonly string _activityName;
        private PerformanceTracker _tracker;

        public TrackPerformanceAttribute(string product, string layer, string activityName)
        {
            this._product = product;
            this._layer = layer;
            this._activityName = activityName;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            var logger = context.HttpContext.RequestServices.GetService(typeof(IWebLogger));
            if (_tracker != null)
                _tracker.Stop();
            else
                Console.WriteLine("Why Null?!");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var activity = $"{request.Path}-{request.Method}";
            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);
            var details = WebLogger.GetLogEntry(_product, _layer, activity, context.HttpContext, dict);

            var logger = context.HttpContext.RequestServices.GetService(typeof(IWebLogger));
            if (logger is WebLogger)
            {
                _tracker = new PerformanceTracker(details, (WebLogger)logger);
            }
            base.OnActionExecuting(context);
        }
    }
}
