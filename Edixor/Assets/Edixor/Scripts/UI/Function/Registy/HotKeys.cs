using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public class HotKeys : FunctionLogic, IFunctionSetting
{
    private KeyActionData[] hotkeys;
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

        HotKeyTab hotKeysTab = new HotKeyTab();
        tabController.AddTab(hotKeysTab);
    }

    public override void Init()
    {
        uiBase = container.Resolve<IUIController>();
        tabController = container.Resolve<ITabController>();
    }

    public void Setting(VisualElement root)
    {
        if (hotkeys == null || hotkeys.Length == 0)
        {
            hotkeys = container.ResolveNamed<HotKeySetting>(ServiceNames.HotKeySetting).GetAllItem();
        }

        foreach (KeyActionData key in hotkeys)
        {
            string combination = string.Join(" + ", key.Combination); 

            Label hotkeyLabel = new Label($"{key.Name}: {combination}");
            Button hotkeyEdit = new Button() { text = "Edit" };

            root.Add(hotkeyLabel);
            root.Add(hotkeyEdit);
        }
    }
}
