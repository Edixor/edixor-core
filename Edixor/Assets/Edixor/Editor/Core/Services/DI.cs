using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewDIContainer", menuName = "Edixor/Services/DI Container", order = 0)]
public class DIContainer : ScriptableObject
{
    private Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();
    private Dictionary<Type, object> singletons = new Dictionary<Type, object>();
    private Dictionary<(Type, string), object> namedInstances = new Dictionary<(Type, string), object>();

    public void Register<T>(T service)
    {
        singletons[typeof(T)] = service;
    }

    public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
    {
        factories[typeof(TInterface)] = () => new TImplementation();
    }

    public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : TInterface, new()
    {
        factories[typeof(TInterface)] = () => 
        {
            if (!singletons.ContainsKey(typeof(TInterface)))
            {
                singletons[typeof(TInterface)] = new TImplementation();
            }
            return singletons[typeof(TInterface)];
        };
    }

    public void RegisterNamed<T>(string name, T service)
    {
        namedInstances[(typeof(T), name)] = service;
    }

    public bool IsRegistered<T>()
    {
        Type type = typeof(T);
        return singletons.ContainsKey(type) || factories.ContainsKey(type);
    }

    public bool IsRegisteredNamed<T>(string name)
    {
        var key = (typeof(T), name);
        return namedInstances.ContainsKey(key);
    }

    public T Resolve<T>()
    {
        Type type = typeof(T);

        if (singletons.TryGetValue(type, out var singletonInstance))
            return (T)singletonInstance;

        if (factories.TryGetValue(type, out var factory))
            return (T)factory();

        throw new Exception($"Dependency of type {type} is not registered in DIContainer.");
    }


    public T ResolveNamed<T>(string name)
    {
        var key = (typeof(T), name);

        if (namedInstances.TryGetValue(key, out var instance))
            return (T)instance;

        throw new Exception($"Named dependency '{name}' of type {typeof(T)} is not registered in DIContainer.");
    }
}
