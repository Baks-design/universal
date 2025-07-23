using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
    public static class Logging
    {
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(object message) => Debug.Log(message);
    }
}