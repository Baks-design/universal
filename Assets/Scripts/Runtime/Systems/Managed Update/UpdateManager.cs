using System;
using UnityEngine;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    public class UpdateManager : MonoBehaviour
    {
        protected static UpdateManager instance;
        static readonly object _lock = new();
        readonly FastRemoveList<IUpdatable> updatableObjects = new();
        readonly FastRemoveList<ILateUpdatable> lateUpdatableObjects = new();
        readonly FastRemoveList<IFixedUpdatable> fixedUpdatableObjects = new();

        public bool HasRegisteredObjects =>
            fixedUpdatableObjects.Count > 0 ||
            updatableObjects.Count > 0 ||
            lateUpdatableObjects.Count > 0;
        public static UpdateManager Instance
        {
            get
            {
                if (ApplicationUtils.IsQuitting)
                    return instance;

                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = FindFirstObjectByType<UpdateManager>();
                        if (instance == null)
                            instance = CreateInstance();
                        else
                            instance.enabled = instance.HasRegisteredObjects;
                    }
                    return instance;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticFields()
        {
            lock (_lock)
            {
                if (instance != null && !Application.isPlaying)
                {
                    instance.Clear();
                    if (instance.gameObject != null)
                        DestroyImmediate(instance.gameObject);
                }
                instance = null;
            }
        }

        static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager))
            {
                hideFlags = HideFlags.DontSave
            };

#if UNITY_EDITOR
            if (!Application.isPlaying)
                gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif

            // Always mark as DontDestroyOnLoad in play mode
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<UpdateManager>();
        }


#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void DestroyEditorUpdateManager()
        {
            if (instance != null && !Application.isPlaying)
            {
                DestroyImmediate(instance.gameObject);
                instance = null;
            }
        }
#endif

        void Awake()
        {
            lock (_lock)
            {
                if (instance == null)
                {
                    instance = this;
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                        hideFlags = HideFlags.HideAndDontSave;
#endif
                    // Ensure persistence in play mode
                    if (Application.isPlaying)
                        DontDestroyOnLoad(gameObject);
                }
                else if (instance != this)
                {
                    DestroyImmediate(gameObject);
                    return;
                }
            }
        }

        void OnDestroy()
        {
            lock (_lock)
            {
                if (instance == this)
                {
                    // Clear all registered objects first
                    Clear();
                    instance = null;
                }
            }
        }
        void FixedUpdate()
        {
            if (!Application.isPlaying && fixedUpdatableObjects.Count == 0)
                return;

            lock (_lock)
            {
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
        }

        void Update()
        {
            if (!Application.isPlaying && updatableObjects.Count == 0)
                return;

            lock (_lock)
            {
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
        }

        void LateUpdate()
        {
            if (!Application.isPlaying && lateUpdatableObjects.Count == 0)
                return;

            lock (_lock)
            {
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
        }

        public void Register(IManagedObject obj)
        {
            if (obj == null) return;

            lock (_lock)
            {
                if (obj is IUpdatable updatable)
                    updatableObjects.Add(updatable);

                if (obj is ILateUpdatable lateUpdatable)
                    lateUpdatableObjects.Add(lateUpdatable);

                if (obj is IFixedUpdatable fixedUpdatable)
                    fixedUpdatableObjects.Add(fixedUpdatable);

                enabled = HasRegisteredObjects;
            }
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null) return;

            lock (_lock)
            {
                if (obj is IUpdatable updatable)
                    updatableObjects.Remove(updatable);

                if (obj is ILateUpdatable lateUpdatable)
                    lateUpdatableObjects.Remove(lateUpdatable);

                if (obj is IFixedUpdatable fixedUpdatable)
                    fixedUpdatableObjects.Remove(fixedUpdatable);

                enabled = HasRegisteredObjects;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                updatableObjects.Clear();
                lateUpdatableObjects.Clear();
                fixedUpdatableObjects.Clear();
                enabled = false;
            }
        }
    }
}
