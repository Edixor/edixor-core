using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

public class Setting : FunctionLogic
{

    private EdixorUIManager uiManager;
    public override void Activate()
    {
        if (uiManager == null)
        {
            Debug.LogError("UIManager is null.");
            return;
        }
        {
            VisualElement parentContainer = uiManager.GetDesign().GetSection("middle-section-content");
            if (parentContainer == null)
            {
                Debug.LogError("ParentContainer is null.");
                return;
            }

            SettingTab settingTab = new SettingTab();
            uiManager.AddTab(settingTab);
        }
    }

    public override void Init()
    {
        uiManager = container.ResolveNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow);
    }
}
