using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class HotKeysTab : EdixorTab
{
    private List<KeyAction> hotkeyActions;
    public HotKeysTab(VisualElement ParentContainer, EdixorWindow window) : base(ParentContainer) {
        hotkeyActions = window.GetSetting().GetHotKeys();
    }

    public override string Title => "HotKey";
    public override string PathUxml => "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uss";

    public override void OnUI() {
        VisualElement designContainer = root.Q<VisualElement>("hotkeys-container");

        foreach (KeyAction key in hotkeyActions)
        {
            string combination = string.Join(" + ", key.Combination); // Объединяем клавиши
            Label hotkeyLabel = new Label($"{key.Name}: {combination}"); // Форматируем строку
            designContainer.Add(hotkeyLabel);
        }
    }

}
