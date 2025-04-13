using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class EdixorDesign {
    public StyleData Style { get; private set; }
    public EdixorLayoutData Layout { get; private set; }
    private VisualTreeAsset tree;
    private StyleSheet layoutSheet;
    private StyleLogic styleLogic;
    private StyleParameters parameters;
    private VisualElement rootElement;
    private IFactory factoryBuilder;
    private DIContainer Container;

    private static Dictionary<string, VisualTreeAsset> visualTreeCache = new Dictionary<string, VisualTreeAsset>();
    private static Dictionary<string, StyleSheet> styleSheetCache = new Dictionary<string, StyleSheet>();

    public EdixorDesign(StyleData style, EdixorLayoutData layout, VisualElement root, DIContainer container)
    {
        Style = style;
        Layout = layout;
        rootElement = root;
        Container = container;
    }

    public EdixorDesign(StyleData style, EdixorLayoutData layout)
    {
        Style = style;
        Layout = layout;
    }

    public void LoadUI(bool demo = false)
    {
        tree = LoadUXML(Layout.PathUxml);

        if (factoryBuilder == null)
            factoryBuilder = Container.Resolve<IFactory>();

        factoryBuilder.Init<EdixorLayoutData, LayoutLogic>(data => data.Logic);
        LayoutLogic layoutLogic = (LayoutLogic)factoryBuilder.CreateLogic(Layout);
        layoutLogic.SetContainer(Container);

        parameters = Style.GetAssetParameter();

        if (demo)
            layoutLogic.DemoInit(rootElement, parameters.FunctionIconColors, parameters.FunctionBackgroundColors);
        else
            layoutLogic.Init(rootElement);

        layoutSheet = LoadStyleSheet(Layout.PathUss);

        styleLogic = new StyleLogic(rootElement, parameters);
        
        parameters = Style.GetAssetParameter();
        
        styleLogic.Init();
        styleLogic.FunctionStyling(layoutLogic.GetFunctions());
    }


    private VisualTreeAsset LoadUXML(string path)
    {
        if (!string.IsNullOrEmpty(PathResolver.ResolvePath(path)))
        {
            if (!visualTreeCache.TryGetValue(path, out tree))
            {
                tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
                if (tree != null)
                {
                    visualTreeCache[path] = tree;
                }
                else
                {
                    Debug.LogError($"Failed to load UXML: {path}");
                    return null;
                }
            }

            rootElement.Clear();
            var instance = tree.Instantiate();
            instance.style.flexGrow = 1;
            instance.style.width = Length.Percent(100);
            instance.style.height = Length.Percent(100);
            rootElement.Add(instance);

            return tree;
        }
        return null;
    }

    private StyleSheet LoadStyleSheet(string path)
    {
        if (!string.IsNullOrEmpty(PathResolver.ResolvePath(path)))
        {
            if (!styleSheetCache.TryGetValue(path, out var cachedSheet))
            {
                cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
                if (cachedSheet != null)
                {
                    styleSheetCache[path] = cachedSheet;
                }
                else
                {
                    Debug.LogError($"Failed to load USS: {path}");
                    return null;
                }
            }

            if (!rootElement.styleSheets.Contains(cachedSheet))
                rootElement.styleSheets.Add(cachedSheet);

            return cachedSheet;
        }
        return null;
    }

    public VisualElement GetSection(string sectionName)
    {
        return rootElement?.Q(sectionName);
    }
}
