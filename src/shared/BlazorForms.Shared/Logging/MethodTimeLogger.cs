using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;

namespace BlazorForms
{
    public static class MethodTimeLogger
    {
        public static ILogger Logger;

        public static void Log(MethodBase methodBase, long milliseconds, string message)
        {
            if (Logger != null)
            {
                Logger.LogInformation($"[{methodBase.DeclaringType.FullName}.{methodBase.Name}] [{milliseconds}ms] {message}");
            }
        }
    }
}
