using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Middlewares
{
    public class ApiKeyMessageHandlerMiddleware
    {
        private string apiKey = "KIWISNOOP";
        private RequestDelegate _next;

        public ApiKeyMessageHandlerMiddleware(RequestDelegate next)
        {
            _next = next;

        }

        public async Task Invoke(HttpContext context)
        {
            if (
                context.Request.Headers.ContainsKey("Authorization")
                && !string.IsNullOrEmpty(context.Request.Headers.ContainsKey("Authorization").ToString())
            )
            {
                if (context.Request.Headers["Authorization"].Equals(apiKey))
                {
                    //TODO:set current user and session helper
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsync("Invalid API Key");
                }
            }
            else
            {
                await _next.Invoke(context);
            }

        }

    }

    public static class MyHandlerExtensions
    {
        public static IApplicationBuilder UseApiKeyMessageHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMessageHandlerMiddleware>();
        }

    }
}
