using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
public class SettingTab : EdixorTab
{
    private EdixorUIManager edixorUIManager;
    private EdixorWindow window;
    private IFunctionSetting activeFunction = null;
    
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
        List<EdixorFunction> functions = edixorUIManager.GetDesign().GetFunctions();

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
            if (version.countVersion == 0)
            {
                version.InitializeVersionActions();
            }
            for (int j = 0; j < version.countVersion; j++)
            {
                Button button = CreateButton(designIndex, j);
                buttonRow.Add(button);
            }
        } else
        {
            Debug.Log("Create button");
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
        VisualElement funcionContainer = root.Q<VisualElement>("funcion-container"); 
        VisualElement funcionSettingContainer = root.Q<VisualElement>("funcion-setting-container"); 

        Label infoLabel = new Label("Click on a function to configure it");
        funcionSettingContainer.Add(infoLabel);

        foreach (EdixorFunction func in functions)
        {
            if (func is IFunctionSetting settingFunc)
            {
                Button function = new Button(() =>
                {
                    if (activeFunction != settingFunc)
                    {

                        if (funcionSettingContainer.Contains(infoLabel))
                        {
                            funcionSettingContainer.Remove(infoLabel);
                        } else
                        {
                            funcionSettingContainer.Clear();
                        }
                        
                        Label functionTitle = new Label(func.Name);
                        functionTitle.AddToClassList("function-title");
                        funcionSettingContainer.Add(functionTitle);

                        if(func.Empty()) {
                            func.Init(window);
                        }

                        settingFunc.Setting(funcionSettingContainer);

                        activeFunction = settingFunc;
                    }
                });

                function.AddToClassList("function-button");

                if (func.Icon != null)
                {
                    Image icon = new Image { image = func.Icon };
                    function.Add(icon);
                }

                funcionContainer.Add(function);
            }
        }
    }
}
