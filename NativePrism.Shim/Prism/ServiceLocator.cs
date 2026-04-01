using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class ServiceLocator
{
    private static readonly ConcurrentDictionary<Type, object> _services = new ConcurrentDictionary<Type, object>();

    public static void Register<T>(T service)
    {
        _services[typeof(T)] = service;
    }

    public static T Resolve<T>()
    {
        if (_services.TryGetValue(typeof(T), out var service))
        {
            return (T)service;
        }
        throw new InvalidOperationException($"Service of type {typeof(T)} not registered.");
    }
}