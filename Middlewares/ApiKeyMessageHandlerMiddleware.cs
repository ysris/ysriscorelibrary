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
            if (context.Request.Headers.ContainsKey("api_key") && context.Request.Headers["api_key"].Equals(apiKey))
                await _next.Invoke(context);
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Invalid API Key");
            }
        }

    }
}
