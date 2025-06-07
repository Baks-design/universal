using System;
using UnityEngine;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public class ManagedUpdateManager : MonoBehaviour
    {
        protected static ManagedUpdateManager instance;
        readonly FastRemoveList<IUpdatable> updatableObjects = new();
        readonly FastRemoveList<ILateUpdatable> lateUpdatableObjects = new();
        readonly FastRemoveList<IFixedUpdatable> fixedUpdatableObjects = new();
        bool isInitialized = false;

        public bool HasRegisteredObjects =>
            fixedUpdatableObjects.Count > 0 ||
            updatableObjects.Count > 0 ||
            lateUpdatableObjects.Count > 0;
        public static ManagedUpdateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<ManagedUpdateManager>();
                    if (instance == null)
                    {
                        Debug.Log("[UpdateManager] Creating new instance automatically");
                        var go = new GameObject(typeof(ManagedUpdateManager).Name + " (Auto-Created)");
                        instance = go.AddComponent<ManagedUpdateManager>();
                        DontDestroyOnLoad(go); // Ensure auto-created instance persists
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            Debug.Log($"[UpdateManager] Awake called on {GetInstanceID()} (Existing instance: {instance != null})");

            switch (instance)
            {
                case null:
                    Debug.Log($"[UpdateManager] Initializing new instance {GetInstanceID()}");
                    instance = this;
                    DontDestroyOnLoad(gameObject);
                    isInitialized = true;
                    enabled = true;
                    break;
                default:
                    if (this != instance)
                    {
                        Debug.LogWarning($"[UpdateManager] Duplicate instance detected {GetInstanceID()}. Destroying.");
                        DestroyImmediate(gameObject);
                        return;
                    }
                    break;
            }

            Debug.Log($"[UpdateManager] Setup complete - Instance ID: {GetInstanceID()}");
        }

        void FixedUpdate()
        {
            if (!isInitialized || (!Application.isPlaying && fixedUpdatableObjects.Count == 0))
                return;

            for (var i = 0; i < fixedUpdatableObjects.Count; i++)
            {
                try
                {
                    var obj = fixedUpdatableObjects[i];
                    if (obj != null)
                        obj.ManagedFixedUpdate(Time.fixedDeltaTime, Time.time);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void Update()
        {
            if (!isInitialized || (!Application.isPlaying && updatableObjects.Count == 0))
                return;

            for (var i = 0; i < updatableObjects.Count; i++)
            {
                try
                {
                    var obj = updatableObjects[i];
                    if (obj != null)
                        obj.ManagedUpdate(Time.deltaTime, Time.time);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void LateUpdate()
        {
            if (!isInitialized || (!Application.isPlaying && lateUpdatableObjects.Count == 0))
                return;

            for (var i = 0; i < lateUpdatableObjects.Count; i++)
            {
                try
                {
                    var obj = lateUpdatableObjects[i];
                    if (obj != null)
                        obj.ManagedLateUpdate(Time.smoothDeltaTime, Time.time);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public void Register(IManagedObject obj)
        {
            if (!isInitialized || obj == null)
            {
                Debug.LogWarning($"[UpdateManager] Attempted to register before initialization or null object");
                return;
            }

            Debug.Log($"[UpdateManager] Registering {obj.GetType().Name}");

            if (obj is IUpdatable updatable)
                updatableObjects.Add(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                lateUpdatableObjects.Add(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                fixedUpdatableObjects.Add(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        public void Unregister(IManagedObject obj)
        {
            if (!isInitialized || obj == null) return;

            Debug.Log($"[UpdateManager] Unregistering {obj.GetType().Name}");

            if (obj is IUpdatable updatable)
                updatableObjects.Remove(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                lateUpdatableObjects.Remove(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                fixedUpdatableObjects.Remove(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        void OnDestroy()
        {
            Debug.Log($"[UpdateManager] OnDestroy called on {GetInstanceID()}");

            if (instance == this)
            {
                Debug.Log("[UpdateManager] Cleaning up manager instance");
                Clear();
                instance = null;
            }
        }

        void Clear()
        {
            updatableObjects.Clear();
            lateUpdatableObjects.Clear();
            fixedUpdatableObjects.Clear();
            enabled = false;
        }
    }
}