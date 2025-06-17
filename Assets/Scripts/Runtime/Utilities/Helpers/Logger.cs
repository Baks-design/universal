using UnityEngine;
using Unity.Logging;

namespace Universal.Runtime.Utilities.Helpers
{
    /// <summary>
    /// A helper class for consistent logging throughout the application
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Supported log levels
        /// </summary>
        public enum LogLevel
        {
            Verbose,
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
            Off
        }
        // Log level configuration
        public static LogLevel CurrentLogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// Log a verbose message (for very detailed debugging)
        /// </summary>
        public static void Verbose(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Verbose)
                Log.Verbose(message, context);
        }

        /// <summary>
        /// Log a debug message (for normal debugging)
        /// </summary>
        public static void Debug(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Debug)
                Log.Debug(message, context);
        }

        /// <summary>
        /// Log an informational message
        /// </summary>
        public static void Info(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Info)
                Log.Info(message, context);
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        public static void Warning(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Warning)
                Log.Warning(message, context);
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        public static void Error(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Error)
                Log.Error(message, context);
        }

        /// <summary>
        /// Log a fatal error message
        /// </summary>
        public static void Fatal(object message, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Fatal)
                Log.Fatal(message, context);
        }

        /// <summary>
        /// Log an exception with stack trace
        /// </summary>
        public static void Exception(System.Exception exception, Object context = null)
        {
            if (CurrentLogLevel <= LogLevel.Error)
                Log.Error($"Exception: {exception.Message}\n{exception.StackTrace}", context);
        }

        /// <summary>
        /// Log a formatted message (string.Format style)
        /// </summary>
        public static void Format(LogLevel level, string format, params object[] args)
        {
            if (CurrentLogLevel <= level)
            {
                var message = string.Format(format, args);
                switch (level)
                {
                    case LogLevel.Verbose:
                        Verbose(message);
                        break;
                    case LogLevel.Debug:
                        Debug(message);
                        break;
                    case LogLevel.Info:
                        Info(message);
                        break;
                    case LogLevel.Warning:
                        Warning(message);
                        break;
                    case LogLevel.Error:
                        Error(message);
                        break;
                    case LogLevel.Fatal:
                        Fatal(message);
                        break;
                }
            }
        }
    }
}
