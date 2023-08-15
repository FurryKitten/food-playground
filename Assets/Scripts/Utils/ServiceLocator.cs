using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    public static ServiceLocator Current { get; private set; }

    private ServiceLocator() { }

    private Dictionary<string, IService> _services = new Dictionary<string, IService>();

    public static void Initialize()
    {
        Current = new ServiceLocator();
    }

    public T Get<T>() where T : IService
    {
        string key = typeof(T).Name;

        if (!_services.ContainsKey(key))
        {
            Debug.LogError($"{key} is not registered with {GetType().Name}");
            throw new InvalidOperationException();
        }

        return (T)_services[key];
    }

    public void Register<T>(T service) where T : IService
    {
        string key = typeof(T).Name;

        if (_services.ContainsKey(key)) {
            Debug.LogError($"{key} is registered with {GetType().Name}");
            return;
        }

        _services.Add(key, service);
    }

    public void Unregister<T>(T service) where T : IService
    {
        string key = typeof(T).Name;

        if (!_services.ContainsKey(key))
        {
            Debug.LogError($"Attempted to unregister service of type {key} " +
                $"which is not registered with the {GetType().Name}.");
            return;
        }

        _services.Remove(key);
    }
}
