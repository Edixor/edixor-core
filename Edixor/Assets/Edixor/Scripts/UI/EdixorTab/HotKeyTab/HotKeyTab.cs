using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class HotKeysTab : EdixorTab
{
    private List<KeyAction> hotkeyActions;
    private VisualElement designContainer;
    private EdixorWindow window;
    public HotKeysTab(VisualElement ParentContainer, EdixorWindow window) : base(ParentContainer) {
        hotkeyActions = window.GetSetting().GetHotKeys();
        this.window = window;
    }

    public override string Title => "HotKey";
    public override string PathUxml => "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uss";

    public override void OnUI() {
        designContainer = root.Q<VisualElement>("hotkeys-container");

        ListHotKeys("Window", 0, 2);
    }

    private void ListHotKeys(string title, int minIndex, int maxIndex) {
        Label titleLabel = new Label(title);
        VisualElement hotkeysContainer = new VisualElement();

        designContainer.Add(hotkeysContainer);

        titleLabel.AddToClassList("title");
        hotkeysContainer.Add(titleLabel);

        for (int i = minIndex; i < maxIndex; i++)
        {
            hotkeysContainer.Add(CreateBoxHotKeys(i));
        }
    }

    private VisualElement CreateBoxHotKeys(int index) {
        VisualElement hotkeysBox = new VisualElement();
        hotkeysBox.AddToClassList("hotkeys-box");

        Label hotkeyName = new Label(hotkeyActions[index].Name);
        hotkeysBox.Add(hotkeyName);

        string combination = string.Join(" + ", hotkeyActions[index].Combination);
        Label hotkeyContent = new Label(combination);
        hotkeysBox.Add(hotkeyContent);

        Button hotkeyEdit = new Button() { text = "Edit" };
        hotkeysBox.Add(hotkeyEdit);

        hotkeyEdit.clicked += () => ShowHotkeyMenu(hotkeyEdit, index);

        return hotkeysBox;
    }

    private void ShowHotkeyMenu(VisualElement target, int index) {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Change Hotkey"), false, () => ChangeHotkey(index));

        if (hotkeyActions[index].enable) {
            menu.AddItem(new GUIContent("Disable Hotkey"), false, () => DisableHotkey(index));
        } else {
            menu.AddItem(new GUIContent("Enable Hotkey"), false, () => EnableHotkey(index));
        }

        Vector2 mousePosition = target.worldBound.position + new Vector2(0, target.worldBound.height);
        menu.DropDown(new Rect(mousePosition, Vector2.zero));
    }



    private void ChangeHotkey(int index) {
        Debug.Log($"Changing hotkey: {hotkeyActions[index].Name}");
    }

    private void EnableHotkey(int index) {
        Debug.Log($"Enabling hotkey: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = true;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }

    private void DisableHotkey(int index) {
        Debug.Log($"Disabling hotkey: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = false;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }
}
