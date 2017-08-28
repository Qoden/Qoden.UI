using System;

namespace Microsoft.Extensions.Logging
{
    public static class LoggerExtensions
    {
        public static void LogError(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogError(default(EventId), ex, message, args);
        }
        
        public static void LogDebug(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogDebug(default(EventId), ex, message, args);
        }
        
        public static void LogWarning(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogWarning(default(EventId), ex, message, args);
        }
        
        public static void LogInformation(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogInformation(default(EventId), ex, message, args);
        }
        
        public static void LogTrace(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogTrace(default(EventId), ex, message, args);
        }
        
        public static void LogCritical(this ILogger logger, Exception ex, string message = null, params object[] args)
        {
            logger.LogCritical(default(EventId), ex, message, args);
        }
    }
}