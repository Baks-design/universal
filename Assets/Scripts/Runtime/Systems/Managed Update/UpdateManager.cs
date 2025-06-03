using System;
using UnityEngine;
using System.Collections.Generic;

namespace Universal.Runtime.Systems.ManagedUpdate
{
    /// <summary>
    /// Singleton MonoBehaviour that calls <see cref="IUpdatable.ManagedUpdate"/>, 
    /// <see cref="ILateUpdatable.ManagedLateUpdate"/> or <see cref="IFixedUpdatable.ManagedFixedUpdate"/> 
    /// to registered objects every frame.
    /// </summary>
    /// <remarks>
    /// Any C# object can be registered for updates, including MonoBehaviours, pure C# classes and structs, 
    /// as long as they implement <see cref="IUpdatable"/>, <see cref="ILateUpdatable"/> or <see cref="IFixedUpdatable"/>.
    /// Managed methods are called inside a try/catch block, so that exceptions don't stop other objects from updating.
    /// <br/>
    /// This class doesn't implement any execution order mechanism, 
    /// so don't rely on managed methods being executed in any order.
    /// In fact, the order of executed methods will most likely change during the lifetime of the UpdateManager.
    /// </remarks>
    [ExecuteAlways]
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
                        // First try to find existing instance in scene
                        instance = FindFirstObjectByType<UpdateManager>();
                        // If none exists, create new one
                        if (instance == null)
                            instance = CreateInstance();
                        else
                            // Ensure existing instance is properly initialized
                            instance.enabled = instance.HasRegisteredObjects;
                    }
                    return instance;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticFields() => instance = null;

        static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager)) { hideFlags = HideFlags.DontSave };
#if UNITYEDITOR
            if (!Application.isPlaying)
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            else
                DontDestroyOnLoad(gameObject);
#endif
            return gameObject.AddComponent<UpdateManager>();
        }

#if UNITYEDITOR
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
            if (instance == null)
            {
                instance = this;
#if UNITYEDITOR
                if (!Application.isPlaying)
                    hideFlags = HideFlags.HideAndDontSave;
                else
                    DontDestroyOnLoad(gameObject);
#endif
            }
            else if (instance != this)
            {
                // If another instance exists, destroy this one
                DestroyImmediate(gameObject);
                return;
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                Clear();
                instance = null;
            }
        }

        void FixedUpdate()
        {
            if (!Application.isPlaying && fixedUpdatableObjects.Count == 0)
                return;

            for (var i = 0; i < fixedUpdatableObjects.Count; i++)
            {
                try
                {
                    fixedUpdatableObjects[i].ManagedFixedUpdate(Time.fixedDeltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void Update()
        {
            if (!Application.isPlaying && updatableObjects.Count == 0)
                return;

            for (var i = 0; i < updatableObjects.Count; i++)
            {
                try
                {
                    updatableObjects[i].ManagedUpdate(Time.deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void LateUpdate()
        {
            if (!Application.isPlaying && lateUpdatableObjects.Count == 0)
                return;

            for (var i = 0; i < lateUpdatableObjects.Count; i++)
            {
                try
                {
                    lateUpdatableObjects[i].ManagedLateUpdate(Time.smoothDeltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public void Register(IManagedObject obj)
        {
            if (obj == null) return;

            if (obj is IUpdatable updatable && !ContainsObject(updatableObjects, updatable))
                updatableObjects.Add(updatable);

            if (obj is ILateUpdatable lateUpdatable && !ContainsObject(lateUpdatableObjects, lateUpdatable))
                lateUpdatableObjects.Add(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable && !ContainsObject(fixedUpdatableObjects, fixedUpdatable))
                fixedUpdatableObjects.Add(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        static bool ContainsObject<T>(IReadOnlyList<T> collection, T item)
        {
            for (var i = 0; i < collection.Count; i++)
                if (EqualityComparer<T>.Default.Equals(collection[i], item))
                    return true;
            return false;
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null) return;

            if (obj is IUpdatable updatable)
                updatableObjects.Remove(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                lateUpdatableObjects.Remove(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                fixedUpdatableObjects.Remove(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        public void Clear()
        {
            updatableObjects.Clear();
            lateUpdatableObjects.Clear();
            fixedUpdatableObjects.Clear();
            enabled = false;
        }
    }
}
