using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLogger.WebExtensions
{
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        private readonly string product;
        private readonly string layer;
        private readonly string activityName;

        public TrackUsageAttribute(string product, string layer, string activityName)
        {
            this.product = product;
            this.layer = layer;
            this.activityName = activityName;
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var dict = new Dictionary<string, object>();
            foreach (var key in context.RouteData.Values?.Keys)
            {
                dict.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);
            }
            var logger = (IWebLogger)context.HttpContext.RequestServices.GetService(typeof(IWebLogger));
            logger.LogUsage(product, layer, activityName, dict, context.HttpContext);
            base.OnActionExecuted(context);
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

        }
    }
}
