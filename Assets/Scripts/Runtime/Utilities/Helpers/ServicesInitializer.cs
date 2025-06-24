#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Universal.Runtime.Utilities.Helpers
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class ServicesInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            //TODO: Add All Services and Changed Manager
        }

#if UNITY_EDITOR
        static ServicesInitializer() => Initialize();
#endif
    }
}
