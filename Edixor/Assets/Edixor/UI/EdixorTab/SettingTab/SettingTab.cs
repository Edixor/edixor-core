using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class SettingTab : EdixorTab
{
    private EdixorUIManager edixorUIManager;
    private VisualElement root;
    public SettingTab(VisualElement ParentContainer, EdixorUIManager edixorUIManager) : base(ParentContainer) {
        this.edixorUIManager = edixorUIManager;
    }

    public override void LoadUI() 
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathUxml);
        if (visualTree == null)
        {
            Debug.LogError("UI file not found.");
            return;
        }

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("'ParentContainer' is null. Cannot load NewTab UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathUss);
        if (styleSheet == null)
        {
            Debug.LogError("StyleSheet file not found.");
            return;
        }
        root.styleSheets.Add(styleSheet);
    }

    public override string Title => "Setting";
    public override string PathUxml => "Assets/Edixor/UI/EdixorTab/SettingTab/SettingTab.uxml";
    public override string PathUss => "Assets/Edixor/UI/EdixorTab/SettingTab/SettingTab.uss";

    public override void OnUI() {

        List<EdixorFunction> functions = edixorUIManager.GetFunctions();

        foreach (EdixorFunction func in functions)
        {
            Debug.Log(func is IFunctionSetting);
            if (func is IFunctionSetting settingFunc)
            {
                settingFunc.Setting(root); 
            }

        }
    }
}
