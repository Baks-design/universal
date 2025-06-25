using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class Logger //TODO: Apply
    { 
        public enum LogLevel
        {
            Info,
            Warning,
            Error,
            Exception
        }

        public static LogLevel CurrentLogLevel = LogLevel.Info;

        public static void Info(string message, Object context = null) => Log(LogLevel.Info, message, context);

        public static void Warn(string message, Object context = null) => Log(LogLevel.Warning, message, context);

        public static void Error(string message, Object context = null) => Log(LogLevel.Error, message, context);

        public static void Exception(Exception ex, Object context = null) => Log(LogLevel.Exception, ex.ToString(), context, ex);

#if UNITY_EDITOR
        public static void Log(LogLevel level, string message, Object context = null, Exception ex = null)
        {
            if (level < CurrentLogLevel) return;

            switch (level)
            {
                case LogLevel.Info:
                    Debug.Log($"[{level}] {message}", context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[{level}] {message}", context);
                    break;
                case LogLevel.Error:
                    Debug.LogError($"[{level}] {message}", context);
                    break;
                case LogLevel.Exception:
                    Debug.LogException(ex, context);
                    break;
            }
        }
#endif
    }
}