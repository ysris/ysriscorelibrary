using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsrisCoreLibrary.Extensions
{
    public static class LoggerExtensions
    {
        public static ILogger LogInformation<T>(this ILogger obj, string msg) where T : class
        {
            obj.LogInformation($"YSRISLOG:{typeof(T)}:{msg}");
            return obj;
        }
    }
}
