using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorUIManager
{
    private List<EdixorFunction> leftFunctions;
    private List<EdixorFunction> rightFunctions;

    private List<EdixorTab> tabs;
    private int indexTab;

    private VisualElement root;
    private readonly EdixorWindow window;

    private VisualElement topSection;
    public VisualElement scrollView;
    private VisualElement leftSection;
    private VisualElement rightSection;

    public EdixorUIManager(EdixorWindow window)
    {
        this.window = window;
    }

    public List<EdixorTab> GetTabs() {
        if (tabs != null && tabs.Count > 0) 
        {
            return tabs;
        }

        return null;
    }
    public List<EdixorFunction> GetFunctions() {
        if (leftFunctions != null && leftFunctions.Count > 0) 
        {
            return leftFunctions;
        }

        return null;
    }

    public void LoadUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Edixor/UI/EdixorWindow/EdixorWIndow.uxml"); //Edixor\UI\EdixorWindow\EdixorWIndow.uxml
        if (visualTree == null)
        {
            Debug.LogError("UI file not found.");
            return;
        }

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        window.rootVisualElement.Clear();
        window.rootVisualElement.Add(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Edixor/UI/EdixorWindow/EdixorWindow.uss");
        if (styleSheet == null)
        {
            Debug.LogError("StyleSheet file not found.");
            return;
        }
        root.styleSheets.Add(styleSheet);

        InitializeSections(root);
    }

    private void InitializeSections(VisualElement root)
    {
        topSection = root.Q<VisualElement>("top-section");
        scrollView = root.Q<VisualElement>("middle-section-content");
        leftSection = root.Q<VisualElement>("left-section");
        rightSection = root.Q<VisualElement>("right-section");

        leftFunctions = new List<EdixorFunction> { new RestartFunction(window) };
        tabs = new List<EdixorTab> { new NewTab(scrollView)};
        rightFunctions = new List<EdixorFunction> { new SettingsFunction(window, this)};

        AddTabs(topSection, scrollView, tabs);

        AddButtonsToSection(leftSection, leftFunctions);

        tabs[indexTab].LoadUI();

        AddButtonsToSection(rightSection, rightFunctions);
    }

    private void AddTabs(VisualElement topSection, VisualElement scrollView, List<EdixorTab> tabs)
    {
        if (topSection == null || tabs == null) return;
        
        int i = 0;
        foreach (EdixorTab tab in tabs)
        { 
            int currentIndex = i;

            Button visualElementTab = new Button(() => SwitchTab(currentIndex))
            {
                text = tab.Title 
            };

            topSection.Add(visualElementTab);

            i++;
        }
    }

    private void AddButtonsToSection(VisualElement section, List<EdixorFunction> functions)
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


    public void AddTab(EdixorTab newTab)
    {
        if (newTab == null) return;

        tabs.Add(newTab);

        int currentIndex = tabs.Count - 1;

        Button visualElementTab = new Button(() => SwitchTab(currentIndex))
        {
            text = newTab.Title 
        };

        topSection.Add(visualElementTab);
        
        SwitchTab(indexTab + 1);
    }

    private void SwitchTab(int index) {
        tabs[indexTab].DeleteUI();
        tabs[index].LoadUI();
        tabs[index].OnUI();
        indexTab = index;
    }
}