using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

public class Factory : IFactory
{
    private Type _dataType;
    private Type _logicType;
    private Type _fullType;
    private Func<ScriptableObject, string> _logicPathSelector;
    private bool _hasFullType;
    private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();
    private static Assembly[] assembliesCache;

    public void Init<TData, TLogic>(Func<TData, string> logicPathSelector)
        where TData : ScriptableObject
        where TLogic : class
    {
        _dataType = typeof(TData);
        _logicType = typeof(TLogic);
        _logicPathSelector = data => logicPathSelector((TData)data);
        _hasFullType = false;
    }

    public void Init<TData, TLogic, TFull>(Func<TData, string> logicPathSelector)
        where TData : ScriptableObject
        where TLogic : class
        where TFull : class
    {
        _dataType = typeof(TData);
        _logicType = typeof(TLogic);
        _fullType = typeof(TFull);
        _logicPathSelector = data => logicPathSelector((TData)data);
        _hasFullType = true;
    }

    public object CreateLogic(ScriptableObject data)
    {
        string logicPath = _logicPathSelector(data);
        logicPath = PathResolver.ResolvePath(logicPath);

        if (string.IsNullOrEmpty(logicPath))
        {
            throw new InvalidOperationException("Не удалось разрешить путь к типу логики.");
        }

        string className = System.IO.Path.GetFileNameWithoutExtension(logicPath);
        Type logicType = FindTypeInAssemblies(className);

        if (logicType == null)
        {
            throw new InvalidOperationException($"Не удалось найти тип логики: {className}");
        }

        return Activator.CreateInstance(logicType);
    }

    private Type FindTypeInAssemblies(string className)
    {
        if (typeCache.TryGetValue(className, out Type cachedType))
            return cachedType;

        if (assembliesCache == null)
            assembliesCache = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assembliesCache)
        {
            var type = assembly.GetTypes().FirstOrDefault(t => t.Name == className);
            if (type != null)
            {
                typeCache[className] = type;
                return type;
            }
        }
        return null;
    }

    public object Create(ScriptableObject data)
    {
        if (_dataType == null || _logicType == null)
            throw new InvalidOperationException("Factory не инициализирована. Вызовите Init() перед использованием.");

        object logic = CreateLogic(data);

        if (!_hasFullType)
            return logic;

        return Activator.CreateInstance(_fullType, data, logic);
    }

    public List<object> CreateAllFromProject()
    {
        if (_dataType == null)
            throw new InvalidOperationException("Factory не инициализирована. Вызовите Init() перед использованием.");

        var dataList = FindAllData();
        return dataList.Select(Create).ToList();
    }

    private List<ScriptableObject> FindAllData()
    {
        return Resources.FindObjectsOfTypeAll(_dataType)
                        .Cast<ScriptableObject>()
                        .ToList();
    }
}
