using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class Logging
    {
        [System.Diagnostics.Conditional("ENABLELOG")]
        public static void Log(object message) => Debug.Log(message);

        [System.Diagnostics.Conditional("ENABLELOG")]
        public static void LogError(object message) => Debug.LogError(message);
        [System.Diagnostics.Conditional("ENABLELOG")]
        public static void LogWarning(object message) => Debug.LogWarning(message);
    }
}