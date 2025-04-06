using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HotKeysTab : EdixorTab
{
    private List<KeyAction> hotkeyActions;
    private VisualElement designContainer;

    // Передаём необходимые данные в базовый конструктор:
    public HotKeysTab(VisualElement ParentContainer, EdixorWindow window)
        : base(ParentContainer, 
               "HotKey", 
               "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uxml", 
               "Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uss")
    {
        this.window = window;
        Init();
    }

    public override void Init(DIContainer container = null) {
        hotkeyActions = container.ResolveNamed<IAdvancedFactoryService<KeyActionData, KeyActionLogica, KeyAction>>(ServiceNames.KeyActionFactory).GetAllData();
        hotKeySetting = container.ResolveNamed<EdixorSetting<HotKeySaveAsset>>(ServiceNames.HotKeySetting);
    }

    /// <summary>
    /// Специфичная логика отображения UI для вкладки HotKeysTab.
    /// Вызывается базовым OnUI() после увеличения openCount.
    /// </summary>
    protected override void OnTabUI()
    {
        if (hotkeyActions == null || hotkeyActions.Count == 0)
        {
            hotkeyActions = window.GetSetting().GetHotKeys();
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

        window.StartHotkeyCapture((List<KeyCode> newCombination) =>
        {
            Debug.Log("Новая комбинация: " + string.Join(" + ", newCombination.Select(k => k.ToString())));
            hotkeyActions[index].Combination.Clear();
            hotkeyActions[index].Combination.AddRange(newCombination);
            window.GetSetting().SetHotKeys(hotkeyActions[index], index);

            Label hotkeyContent = designContainer.Q<Label>("hotkeyContent" + index);
            if (hotkeyContent != null)
            {
                hotkeyContent.text = string.Join(" + ", hotkeyActions[index].Combination.Select(k => k.ToString()));
            }
        });
    }

    private void EnableHotkey(int index)
    {
        Debug.Log($"Включение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = true;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }

    private void DisableHotkey(int index)
    {
        Debug.Log($"Отключение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = false;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }
}
