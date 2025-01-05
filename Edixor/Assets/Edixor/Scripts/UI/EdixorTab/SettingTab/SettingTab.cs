using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
public class SettingTab : EdixorTab
{
    private EdixorUIManager edixorUIManager;
    private EdixorWindow window;
    private VisualElement root;
    
    public SettingTab(VisualElement ParentContainer, EdixorWindow window) : base(ParentContainer)
    {
        this.edixorUIManager = window.GetUIManager();
        this.window = window;
    }

    public override void LoadUI()
    {
        var visualTree = LoadAssetAtPath<VisualTreeAsset>(PathUxml);
        if (visualTree == null) return;

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("ParentContainer is null. Cannot load NewTab UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);

        var styleSheet = LoadAssetAtPath<StyleSheet>(PathUss);
        if (styleSheet == null) return;

        root.styleSheets.Add(styleSheet);
    }

    public override string Title => "Setting";
    public override string PathUxml => "Assets/Edixor/Scripts/UI/EdixorTab/SettingTab/SettingTab.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/EdixorTab/SettingTab/SettingTab.uss";

    public override void OnUI()
    {
        // Получаем список функций
        List<EdixorFunction> functions = edixorUIManager.GetDesign().GetFunctions();

        // Получаем контейнер для добавления элементов
        VisualElement designContainer = root.Q<VisualElement>("design-container");
        if (designContainer == null)
        {
            Debug.LogError("Design container not found.");
            return;
        }

        AddDesignsToContainer(designContainer);
        AddFunctionSettings(functions);
    }

    private void AddDesignsToContainer(VisualElement designContainer)
    {
        int designCount = window.GetSetting().GetDesigns().Count;
        for (int i = 0; i < designCount; i++)
        {
            VisualElement designBox = CreateDesignBox(i);
            designContainer.Add(designBox);
        }
    }

    private VisualElement CreateDesignBox(int designIndex)
    {
        VisualElement designBox = new VisualElement();
        designBox.AddToClassList("design-box");
        designBox.Add(new Label(window.GetSetting().GetCurrentDesign(designIndex).Name));

        VisualElement buttonRow = new VisualElement();
        buttonRow.AddToClassList("button-row");
        AddButtonsToRow(buttonRow, designIndex);

        designBox.Add(buttonRow);
        designBox.Add(new Label(designIndex == window.GetSetting().GetDesignIndex() ? "selections" : "not selected"));
        designBox.Add(new Label(window.GetSetting().GetCurrentDesign(designIndex).Description));

        return designBox;
    }

    private void AddButtonsToRow(VisualElement buttonRow, int designIndex)
    {
        EdixorDesign design = window.GetSetting().GetCurrentDesign(designIndex);
        if (design is IVersions version)
        {
            for (int j = 0; j < version.countVersion; j++)
            {
                Button button = CreateButton(designIndex, j);
                buttonRow.Add(button);
            }
        } else
        {
            Button button = CreateButton(designIndex, 0);
            buttonRow.Add(button);
        }
    }

    private Button CreateButton(int designIndex, int buttonIndex)
    {
        Button button = new Button(() =>
        {
            window.GetSetting().SetDesignIndex(designIndex, buttonIndex);
            window.RestartWindow();
        });

        button.AddToClassList("image-button");
        return button;
    }

    private void AddFunctionSettings(List<EdixorFunction> functions)
    {
        foreach (EdixorFunction func in functions)
        {
            if (func is IFunctionSetting settingFunc)
            {
                settingFunc.Setting(root);
            }
        }
    }
}
