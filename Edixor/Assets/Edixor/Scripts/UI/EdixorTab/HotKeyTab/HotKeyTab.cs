using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

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
        ListHotKeys("Window", 0, hotkeyActions.Count);
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

        // Получаем строку для отображения комбинации
        string combination = string.Join(" + ", hotkeyActions[index].Combination.Select(k => k.ToString()));
        Label hotkeyContent = new Label(combination);
        // Присвоим уникальное имя (если нужно для поиска через Q)
        hotkeyContent.name = "hotkeyContent" + index;
        hotkeysBox.Add(hotkeyContent);

        Button hotkeyEdit = new Button() { text = "Edit" };
        hotkeysBox.Add(hotkeyEdit);

        // При нажатии передаём метку, которую будем обновлять в callback
        hotkeyEdit.clicked += () => ChangeHotkey(index, hotkeyContent);

        return hotkeysBox;
    }

    /// <summary>
    /// Метод для изменения горячей клавиши.
    /// При завершении захвата обновляется и отображение комбинации.
    /// </summary>
    private void ChangeHotkey(int index, Label hotkeyContent)
    {
        Debug.Log($"Изменение горячей клавиши: {hotkeyActions[index].Name}");

        window.StartHotkeyCapture((List<KeyCode> newCombination) =>
        {
            Debug.Log("Новая комбинация: " + string.Join(" + ", newCombination.Select(k => k.ToString())));

            hotkeyActions[index].Combination.Clear();
            hotkeyActions[index].Combination.AddRange(newCombination);
            window.GetSetting().SetHotKeys(hotkeyActions[index], index);

            // Обновляем отображение новой комбинации динамически.
            hotkeyContent.text = string.Join(" + ", hotkeyActions[index].Combination.Select(k => k.ToString()));
        });
    }

    private void EnableHotkey(int index) {
        Debug.Log($"Включение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = true;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }

    private void DisableHotkey(int index) {
        Debug.Log($"Отключение горячей клавиши: {hotkeyActions[index].Name}");
        hotkeyActions[index].enable = false;
        window.GetSetting().SetHotKeys(hotkeyActions[index], index);
    }
}
