using System;
using System.Collections.Generic;

using ExTools;

public static class TabRegistry
{
    private static readonly Dictionary<string, Type> _nameToType = new();

    public static void RegisterTab<T>() where T : EdixorTab
    {
        string key = typeof(T).Name;
        if (!_nameToType.ContainsKey(key))
            _nameToType[key] = typeof(T);
    }

    public static bool TryGetType(string name, out Type type) => _nameToType.TryGetValue(name, out type);

    public static IEnumerable<string> GetAllRegisteredTabNames() => _nameToType.Keys;
}
