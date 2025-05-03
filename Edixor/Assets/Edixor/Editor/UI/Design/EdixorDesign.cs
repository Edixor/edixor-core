using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

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

        tree = Layout.LayoutVisualTreeAsset;
        layoutSheet = Layout.LayoutStyleSheet;

        if (tree == null)
        {
            Debug.LogError("[EdixorDesign] LayoutData is missing VisualTreeAsset (uxml).");
            return;
        }

        if (layoutSheet == null)
        {
            Debug.LogWarning("[EdixorDesign] LayoutData is missing StyleSheet (uss). Proceeding without styles.");
        }

        // Instantiate UI
        rootElement.Clear();
        var instance = tree.Instantiate();
        instance.style.flexGrow = 1;
        instance.style.width = Length.Percent(100);
        instance.style.height = Length.Percent(100);
        rootElement.Add(instance);

        // Apply USS
        if (layoutSheet != null && !rootElement.styleSheets.Contains(layoutSheet))
            rootElement.styleSheets.Add(layoutSheet);

        parameters = (EdixorParameters)Style.AssetParameters[0];

        styleLogic = new StyleLogic(rootElement, parameters);
        styleLogic.Init();
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
