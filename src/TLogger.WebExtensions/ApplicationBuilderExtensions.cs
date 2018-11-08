using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TLogger.WebExtensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureTLoggerServices(this IServiceCollection serviceCollection)
        {
            IWebLogger wl = new WebLogger();
            serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            serviceCollection.AddSingleton(typeof(IWebLogger), wl);
        }
        public static void EncapsulateExceptions(this IApplicationBuilder app, string product, string layer,string defMessage)
        {


            app.UseExceptionHandler(eApp =>
            {
                eApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    IWebLogger logger = (IWebLogger)context.RequestServices.GetService(typeof(IWebLogger));
                    var errorCtx = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorCtx != null)
                    {
                        var ex = errorCtx.Error;

                        logger.LogException(product, layer, null, ex, null, context);

                        var errorId = Activity.Current?.Id ?? context.TraceIdentifier;
                        var jsonResponse = JsonConvert.SerializeObject(new CustomErrorResponse
                        {
                            ErrorId = errorId,
                            Message = defMessage
                        });
                        await context.Response.WriteAsync(jsonResponse, Encoding.UTF8);
                    }
                });
            });

        }
    }
}
