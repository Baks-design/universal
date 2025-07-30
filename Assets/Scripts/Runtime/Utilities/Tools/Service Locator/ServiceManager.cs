﻿using System;
using System.Collections.Generic;
using Universal.Runtime.Utilities.Helpers;

namespace Universal.Runtime.Utilities.Tools.ServicesLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> services = new();

        public IEnumerable<object> RegisteredServices => services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }
            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (services.TryGetValue(type, out object obj)) return obj as T;
            throw new ArgumentException($"ServiceManager.Get: Service of type {type.FullName} not registered");
        }

        public ServiceManager Register<T>(T service)
        {
            var type = typeof(T);
            if (!services.TryAdd(type, service))
                Logging.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            if (!services.TryAdd(type, service))
                Logging.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");
            return this;
        }
    }
}