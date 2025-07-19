using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Register : IRegister
{
    private Type _dataType;
    private static Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
    private static Assembly[] _assembliesCache;

    public void Init<TData>() where TData : ScriptableObject
    {
        _dataType = typeof(TData);
        if (_assembliesCache == null)
            _assembliesCache = AppDomain.CurrentDomain.GetAssemblies();
    }

    public Type RegisterType(string className)
    {
        if (_typeCache.TryGetValue(className, out Type cachedType))
            return cachedType;

        foreach (var assembly in _assembliesCache)
        {
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
            if (type != null)
            {
                _typeCache[className] = type;
                return type;
            }
        }
        return null;
    }

    public List<Type> GetRegisteredTypes()
    {
        return _typeCache.Values.ToList();
    }
}
