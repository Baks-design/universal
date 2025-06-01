using System;
using System.Linq;
using UnityEngine;
using Universal.Runtime.Systems.Update.Interface;
using Universal.Runtime.Systems.Update.Utilities;
using Universal.Runtime.Systems.Update.Internal;

namespace Universal.Runtime.Systems.Update
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
        protected static UpdateManager _instance;
        static readonly object _lock = new();
        readonly FastRemoveList<IUpdatable> _updatableObjects = new();
        readonly FastRemoveList<ILateUpdatable> _lateUpdatableObjects = new();
        readonly FastRemoveList<IFixedUpdatable> _fixedUpdatableObjects = new();

        public bool HasRegisteredObjects =>
            _fixedUpdatableObjects.Count > 0 ||
            _updatableObjects.Count > 0 ||
            _lateUpdatableObjects.Count > 0;
        public static UpdateManager Instance
        {
            get
            {
                if (ApplicationUtils.IsQuitting)
                    return _instance;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // First try to find existing instance in scene
                        _instance = FindFirstObjectByType<UpdateManager>();
                        // If none exists, create new one
                        if (_instance == null)
                            _instance = CreateInstance();
                        else
                            // Ensure existing instance is properly initialized
                            _instance.enabled = _instance.HasRegisteredObjects;
                    }
                    return _instance;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStaticFields() => _instance = null;

        static UpdateManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(UpdateManager)) { hideFlags = HideFlags.DontSave };
#if UNITY_EDITOR
            if (!Application.isPlaying)
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            else
                DontDestroyOnLoad(gameObject);
#endif
            return gameObject.AddComponent<UpdateManager>();
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void DestroyEditorUpdateManager()
        {
            if (_instance != null && !Application.isPlaying)
            {
                DestroyImmediate(_instance.gameObject);
                _instance = null;
            }
        }
#endif

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    hideFlags = HideFlags.HideAndDontSave;
                else
                    DontDestroyOnLoad(gameObject);
#endif
            }
            else if (_instance != this)
            {
                // If another instance exists, destroy this one
                DestroyImmediate(gameObject);
                return;
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                Clear();
                _instance = null;
            }
        }

        void FixedUpdate()
        {
            if (!Application.isPlaying && _fixedUpdatableObjects.Count == 0)
                return;

            var deltaTime = Time.fixedDeltaTime;

            for (var i = 0; i < _fixedUpdatableObjects.Count; i++)
            {
                try
                {
                    _fixedUpdatableObjects[i].ManagedFixedUpdate(deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void Update()
        {
            if (!Application.isPlaying && _updatableObjects.Count == 0)
                return;

            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _updatableObjects.Count; i++)
            {
                try
                {
                    _updatableObjects[i].ManagedUpdate(deltaTime);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        void LateUpdate()
        {
            if (!Application.isPlaying && _lateUpdatableObjects.Count == 0)
                return;

            var deltaTime = Time.smoothDeltaTime;

            for (var i = 0; i < _lateUpdatableObjects.Count; i++)
            {
                try
                {
                    _lateUpdatableObjects[i].ManagedLateUpdate(deltaTime);
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

            if (obj is IUpdatable updatable && !_updatableObjects.Contains(updatable))
                _updatableObjects.Add(updatable);

            if (obj is ILateUpdatable lateUpdatable && !_lateUpdatableObjects.Contains(lateUpdatable))
                _lateUpdatableObjects.Add(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable && !_fixedUpdatableObjects.Contains(fixedUpdatable))
                _fixedUpdatableObjects.Add(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        public void Unregister(IManagedObject obj)
        {
            if (obj == null) return;

            if (obj is IUpdatable updatable)
                _updatableObjects.Remove(updatable);

            if (obj is ILateUpdatable lateUpdatable)
                _lateUpdatableObjects.Remove(lateUpdatable);

            if (obj is IFixedUpdatable fixedUpdatable)
                _fixedUpdatableObjects.Remove(fixedUpdatable);

            enabled = HasRegisteredObjects;
        }

        public void Clear()
        {
            _updatableObjects.Clear();
            _lateUpdatableObjects.Clear();
            _fixedUpdatableObjects.Clear();
            enabled = false;
        }
    }
}
