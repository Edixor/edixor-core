using UnityEditor;
using System;
using System.Linq;
using System.Reflection;

using ExTools;

public static class TabRegistrationBootstrap
{
    public static void RegisterTabs()
    {
        var types = TypeCache.GetTypesDerivedFrom<EdixorTab>();
        foreach (var type in types)
        {
            if (type.IsAbstract || type.ContainsGenericParameters)
                continue;
            MethodInfo method = typeof(TabRegistry)
                .GetMethod(nameof(TabRegistry.RegisterTab), BindingFlags.Public | BindingFlags.Static);
            MethodInfo generic = method.MakeGenericMethod(type);
            generic.Invoke(null, null);
        }
    }
}
