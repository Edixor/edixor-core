using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HotKeysTab : EdixorTab
{
    private HotKeyService hotKeySetting;
    private List<KeyActionData> hotkeyActions;
    private VisualElement designContainer;

    public void OnEnable()
    {
        Debug.Log("HotKeysTab OnEnable called.");
        // Логика для OnEnable
    }

    public void Awake()
    {
        Debug.Log("HotKeysTab Awake called.");
        // Логика для Awake
    }

    public void OnDisable()
    {
        Debug.Log("HotKeysTab OnDisable called.");
        // Логика для OnDisable
    }

    protected void Start()
    {
        if (hotkeyActions == null || hotkeyActions.Count == 0)
        {
  
        }

        designContainer = root.Q<VisualElement>("hotkeys-container");
        ListHotKeys("Window", 0, hotkeyActions.Count);
    }

    private void ListHotKeys(string title, int minIndex, int maxIndex)
    {
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

    private VisualElement CreateBoxHotKeys(int index)
    {
        VisualElement hotkeysBox = new VisualElement();
        hotkeysBox.AddToClassList("hotkeys-box");

        // Имя действия
        Label hotkeyName = new Label(hotkeyActions[index].Name);
        hotkeysBox.Add(hotkeyName);

        // Отображение комбинации (преобразуем список KeyCode в строку)
        string combination = string.Join(" + ", hotkeyActions[index].Combination.Select(k => k.ToString()));
        Label hotkeyContent = new Label(combination);
        hotkeyContent.name = "hotkeyContent" + index; // Уникальное имя для поиска через Q
        hotkeysBox.Add(hotkeyContent);

        // Кнопка редактирования
        Button hotkeyEdit = new Button() { text = "Edit" };
        hotkeysBox.Add(hotkeyEdit);

        hotkeyEdit.clicked += () => ShowHotkeyMenu(hotkeysBox, index);

        return hotkeysBox;
    }

    private void ShowHotkeyMenu(VisualElement target, int index)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Change Hotkey"), false, () => ChangeHotkey(index));
        if (hotkeyActions[index].enable)
        {
            menu.AddItem(new GUIContent("Disable Hotkey"), false, () => DisableHotkey(index));
        }
        else
        {
            menu.AddItem(new GUIContent("Enable Hotkey"), false, () => EnableHotkey(index));
        }

        Vector2 mousePosition = target.worldBound.position + new Vector2(0, target.worldBound.height);
        menu.DropDown(new Rect(mousePosition, Vector2.zero));
    }

    private void ChangeHotkey(int index)
    {
        Debug.Log($"Изменение горячей клавиши: {hotkeyActions[index].Name}");
        // Логика для изменения горячей клавиши
    }

    private void EnableHotkey(int index)
    {
        Debug.Log($"Включение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = true;
        hotKeySetting.GetSettings().SaveItems[index] = hotkeyActions[index];
    }

    private void DisableHotkey(int index)
    {
        Debug.Log($"Отключение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = false;
        hotKeySetting.GetSettings().SaveItems[index] = hotkeyActions[index];
    }
}
