using UnityEngine;
using Universal.Runtime.Systems.ManagedUpdate;

namespace Universal.Runtime.Utilities.Tools
{
    public class PersistentSingleton<T> : AManagedBehaviour where T : Component //TODO: Changed to ServiceLocator
    {
        protected static T instance;
        protected bool UnparentOnAwake = true;

        public static bool HasInstance => instance != null;
        public static T Current => instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();
                    if (instance == null)
                        instance = new GameObject(typeof(T).Name + "AutoCreated").AddComponent<T>();
                }
                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingleton();

        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
                return;

            if (UnparentOnAwake)
                transform.SetParent(null);

            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
                enabled = true;
            }
            else if (this != instance)
                Destroy(gameObject);
        }
    }
}