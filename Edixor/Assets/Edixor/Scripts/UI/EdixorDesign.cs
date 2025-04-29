using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;

public class EdixorDesign {
    public StyleData Style { get; private set; }
    public LayoutData Layout { get; private set; }
    private VisualTreeAsset tree;
    private StyleSheet layoutSheet;
    private StyleLogic styleLogic;
    private EdixorParameters parameters;
    private VisualElement rootElement;
    private LayoutLogic layoutLogic;
    private DIContainer Container;

    private static Dictionary<string, VisualTreeAsset> visualTreeCache = new Dictionary<string, VisualTreeAsset>();
    private static Dictionary<string, StyleSheet> styleSheetCache = new Dictionary<string, StyleSheet>();

    public EdixorDesign(StyleData style, LayoutData layout, VisualElement root, DIContainer container)
    {
        Style = style;
        Layout = layout;
        rootElement = root;
        Container = container;
    }

    public EdixorDesign(StyleData style, LayoutData layout)
    {
        Style = style;
        Layout = layout;
    }

    public EdixorDesign(DIContainer container) {
        Container = container;
    }

    public void LoadUI(bool demo = false)
    {
        if (rootElement == null)
            rootElement = new VisualElement();

        // 1) Проверьте, что путь резолвится:
        var uxmlResolved = PathResolver.ResolvePath(Layout.PathUxml);
        Debug.Log($"[EdixorDesign] Resolved UXML path: {uxmlResolved}");

        // 2) Загрузили ли мы дерево?
        tree = LoadUXML(Layout.PathUxml);
        Debug.Log($"[EdixorDesign] UXML loaded: {tree != null}");

        // 3) Аналогично для USS:
        var ussResolved = PathResolver.ResolvePath(Layout.PathUss);
        Debug.Log($"[EdixorDesign] Resolved USS path: {ussResolved}");
        layoutSheet = LoadStyleSheet(Layout.PathUss);
        Debug.Log($"[EdixorDesign] USS loaded: {layoutSheet != null}");

        // Если хоть одна из вещей null — смотрите, почему PathResolver.ResolvePath
        // не возвращает корректный путь, или файл лежит не по тому адресу.

        parameters = (EdixorParameters)Container
            .ResolveNamed<StyleService>(ServiceNames.StyleSetting)
            .GetStyleParameter<EdixorParameters>();

        styleLogic = new StyleLogic(rootElement, parameters);
        styleLogic.Init();
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

    public VisualElement GetRoot()
    {
        return rootElement;
    }
}
