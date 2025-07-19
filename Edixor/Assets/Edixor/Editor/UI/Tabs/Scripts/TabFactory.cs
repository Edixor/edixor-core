using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using ExTools;


public class TabFactory
{
    private readonly Dictionary<string, Type> _registeredTabs = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterTabsFromLocator(DIContainer container)
    {
        var allTabs = EdixorObjectLocator
            .FindAndCreateInstances<MonoScript, EdixorTab>("Tabs")
            .Distinct()
            .ToList();

        foreach (var tabInstance in allTabs)
        {
            var type = tabInstance.GetType();
            string tabName = type.Name; 
            if (!_registeredTabs.ContainsKey(tabName))
                _registeredTabs[tabName] = type;
        }
    }

    public bool TryCreate(string tabName, TabData data, DIContainer container, VisualElement root, out EdixorTab EdixorTab)
    {
        EdixorTab = null;
        if (!_registeredTabs.TryGetValue(tabName, out var type))
            return false;
        EdixorTab = (EdixorTab)Activator.CreateInstance(type);
        EdixorTab.Initialize(data, container, root);
        return true;
    }
}
