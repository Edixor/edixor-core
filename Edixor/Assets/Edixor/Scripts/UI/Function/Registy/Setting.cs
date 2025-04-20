using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

public class Setting : FunctionLogic
{

    private ITabController tabController;
    private IUIController uiBase;
    public override void Activate()
    {
        if (tabController == null)
        {
            Debug.LogError("TabController is null.");
            return;
        }
        if (uiBase == null)
        {
            Debug.LogError("UIController is null.");
            return;
        }
        VisualElement parentContainer = uiBase.GetElement("middle-section-content");
        if (parentContainer == null)
        {
            Debug.LogError("ParentContainer is null.");
            return;
        }

        SettingTab settingTab = new SettingTab();
        tabController.AddTab(settingTab);
    }

    public override void Init()
    {
        uiBase = container.Resolve<IUIController>();
        tabController = container.Resolve<ITabController>();
    }
}
