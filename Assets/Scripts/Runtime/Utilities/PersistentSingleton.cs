using UnityEngine;
using Universal.Runtime.Systems.Update;

namespace Universal.Runtime.Utilities
{
    public class PersistentSingleton<T> : AManagedBehaviour where T : Component
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
                    {
                        instance = new GameObject(typeof(T).Name + "AutoCreated").AddComponent<T>();
                    }
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
            {
                Destroy(gameObject);
            }
        }
    }
}