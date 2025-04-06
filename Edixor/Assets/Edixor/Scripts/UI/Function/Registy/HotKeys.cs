using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public class HotKeys : FunctionLogica, IFunctionSetting
{
    private List<KeyActionData> hotkeys;
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

            HotKeysTab hotKeysTab = new HotKeysTab();
            uiManager.AddTab(hotKeysTab);
        }
    }

    public override void Init()
    {
        uiManager = container.ResolveNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow);
    }

    public void Setting(VisualElement root)
    {
        if (hotkeys == null || hotkeys.Count == 0)
        {
            hotkeys = container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting).GetHotKeys();
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
