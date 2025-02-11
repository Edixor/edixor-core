using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[Serializable]
public abstract class EdixorDesign
{
    protected VisualElement root;
    protected List<EdixorFunction> functions;
    protected EdixorWindow window;

    public EdixorDesign(EdixorWindow window)
    {
        this.window = window;
    }

    public abstract string PathUxml { get; }
    public abstract string PathUss { get; }
    public abstract string Image { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }


    public virtual void LoadUI() {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathUxml); 
        if (visualTree == null)
        {
            Debug.LogError("UI file not found.");
            return;
        }
        
        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        window.rootVisualElement.Clear();
        window.rootVisualElement.Add(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUss);
        if (styleSheet == null)
        {
            Debug.LogError("StyleSheet file not found.");
            return;
        }
        root.styleSheets.Add(styleSheet);

        functions = window.GetSetting().GetFunctions();

        InitializeSections(root);
    }

    protected void SwapSections(VisualElement first, VisualElement second, bool justifyContentRevers = false)
    {
        var parent = first?.parent;
        if (parent == null || second == null)
        {
            Debug.LogWarning("Failed to find parent or sections for swapping.");
            return;
        }

        int firstIndex = parent.IndexOf(first);
        int secondIndex = parent.IndexOf(second);

        if (firstIndex < 0 || secondIndex < 0) return;

        parent.Remove(first);
        parent.Remove(second);

        parent.Insert(firstIndex, second);
        parent.Insert(secondIndex, first);

        if (justifyContentRevers) return;

        first.style.justifyContent = Justify.FlexEnd;
        second.style.justifyContent = Justify.FlexStart;
    }

    protected abstract void InitializeSections(VisualElement root);

    public VisualElement GetSection(string sectionName)
    {
        return root?.Q<VisualElement>(sectionName);
    }

    public EdixorFunction GetFunction(string functionName)
    {
        foreach (EdixorFunction func in functions)
        {
            if (func.Name == functionName)
            {
                return func;
            }
        }

        return null;
    }

    protected List<EdixorFunction> SeparationоfFunctions(Type[] types)
    {
        var functionsList = new List<EdixorFunction>();

        foreach (var type in types)
        {
            if (!typeof(EdixorFunction).IsAssignableFrom(type))
            {
                Debug.LogError($"Type {type.Name} is not a subclass of EdixorFunction.");
                continue;
            }

            try
            {
                var constructor = type.GetConstructor(new[] { typeof(EdixorWindow) });
                if (constructor == null)
                {
                    Debug.LogError($"Type {type.Name} does not have a constructor accepting EdixorWindow.");
                    continue;
                }

                var instance = constructor.Invoke(new object[] { window }) as EdixorFunction;
                if (instance != null)
                {
                    functionsList.Add(instance);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error creating instance for type {type.Name}: {ex.Message}");
            }
        }

        return functionsList;
    }

    protected void AddButtonsToSection(VisualElement section, List<EdixorFunction> functions)
    {
        if (section == null || functions == null) return;

        foreach (var func in functions)
        {
            Button button = new Button(() => func.Activate());

            if (func.Icon != null)
            {
                Image icon = new Image { image = func.Icon };
                button.Add(icon);
            }

            section.Add(button);
        }
    }

    public List<EdixorFunction> GetFunctions()
    {
        return functions;
    }
}

