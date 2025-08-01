﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Utilities.Tools.ServicesLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        static Dictionary<Scene, ServiceLocator> sceneContainers;
        static List<GameObject> tmpSceneGameObjects;
        readonly ServiceManager services = new();
        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (global == this)
                Logging.LogWarning("ServiceLocator.ConfigureAsGlobal: Already configured as global");
            else if (global != null)
                Logging.LogError("ServiceLocator.ConfigureAsGlobal: Another ServiceLocator is already configured as global");
            else
            {
                global = this;
                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
        }

        internal void ConfigureForScene()
        {
            var scene = gameObject.scene;
            if (sceneContainers.ContainsKey(scene))
            {
                Logging.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene");
                return;
            }
            sceneContainers.Add(scene, this);
        }

        /// <summary>
        /// Gets the global ServiceLocator instance. Creates new if none exists.
        /// </summary>        
        public static ServiceLocator Global
        {
            get
            {
                if (global != null) return global;

                if (FindFirstObjectByType<ServiceLocatorGlobal>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                var container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobal>().BootstrapOnDemand();

                return global;
            }
        }

        /// <summary>
        /// Returns the <see cref="ServiceLocator"/> configured for the scene of a MonoBehaviour. Falls back to the global instance.
        /// </summary>
        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            var scene = mb.gameObject.scene;

            if (sceneContainers.TryGetValue(scene, out var container) && container != mb) return container;

            tmpSceneGameObjects.Clear();
            scene.GetRootGameObjects(tmpSceneGameObjects);

            for (var i = 0; i < tmpSceneGameObjects.Count; i++)
            {
                var go = tmpSceneGameObjects[i];

                if (go.GetComponent<ServiceLocatorScene>() == null) continue;

                if (go.TryGetComponent(out ServiceLocatorScene bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;
        }

        /// <summary>
        /// Gets the closest ServiceLocator instance to the provided 
        /// MonoBehaviour in hierarchy, the ServiceLocator for its scene, or the global ServiceLocator.
        /// </summary>
        public static ServiceLocator For(MonoBehaviour mb)
        {
            var parentLocator = mb.GetComponentInParent<ServiceLocator>().OrNull();
            if (parentLocator != null) return parentLocator;

            var sceneLocator = ForSceneOf(mb);
            if (sceneLocator != null) return sceneLocator;

            return Global;
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using the service's type.
        /// </summary>
        /// <param name="service">The service to register.</param>  
        /// <typeparam name="T">Class type of the service to be registered.</typeparam>
        /// <returns>The ServiceLocator instance after registering the service.</returns>
        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }

        /// <summary>
        /// Registers a service to the ServiceLocator using a specific type.
        /// </summary>
        /// <param name="type">The type to use for registration.</param>
        /// <param name="service">The service to register.</param>  
        /// <returns>The ServiceLocator instance after registering the service.</returns>
        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type, service);
            return this;
        }

        /// <summary>
        /// Gets a service of a specific type. If no service of the required type is found, an error is thrown.
        /// </summary>
        /// <param name="service">Service of type T to get.</param>  
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <returns>The ServiceLocator instance after attempting to retrieve the service.</returns>
        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>
        /// Allows retrieval of a service of a specific type. An error is thrown if the required service does not exist.
        /// </summary>
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <returns>Instance of the service of type T.</returns>
        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (TryGetService(type, out T service)) return service;

            if (TryGetNextInHierarchy(out ServiceLocator container)) return container.Get<T>();

            throw new ArgumentException($"Could not resolve type '{typeof(T).FullName}'.");
        }

        /// <summary>
        /// Tries to get a service of a specific type. Returns whether or not the process is successful.
        /// </summary>
        /// <param name="service">Service of type T to get.</param>  
        /// <typeparam name="T">Class type of the service to be retrieved.</typeparam>
        /// <returns>True if the service retrieval was successful, false otherwise.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (TryGetService(type, out service)) return true;

            return TryGetNextInHierarchy(out ServiceLocator container) && container.TryGet(out service);
        }

        bool TryGetService<T>(out T service) where T : class => services.TryGet(out service);

        bool TryGetService<T>(Type _, out T service) where T : class => services.TryGet(out service);

        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == global)
            {
                container = null;
                return false;
            }

            var parentTransform = transform.parent;
            if (parentTransform != null)
            {
                container = parentTransform.GetComponentInParent<ServiceLocator>().OrNull();
                if (container != null) return true;
            }

            container = ForSceneOf(this);
            return container != null;
        }

        void OnDestroy()
        {
            if (this == global)
                global = null;
            else if (sceneContainers != null && sceneContainers.ContainsValue(this))
                sceneContainers.Remove(gameObject.scene);
        }

        // https://docs.unity3d.com/ScriptReference/RuntimeInitializeOnLoadMethodAttribute.html
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tmpSceneGameObjects = new List<GameObject>();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal() => new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobal));

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene() => new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorScene));
#endif
    }
}