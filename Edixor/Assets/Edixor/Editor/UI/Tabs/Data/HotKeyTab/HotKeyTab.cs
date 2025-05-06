using HotKeySettingEntries = ISettingEntries<HotKeySettingsAsset.KeyActionDictionaryEntry, string>;

using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ExTools;
using System;

[Serializable]
public class HotKeyTab : EdixorTab
{
    private HotKeySettingEntries hotKeySettingEntries;
    private List<HotKeySettingsAsset.KeyActionDictionaryEntry> hotkeyEntries;
    private VisualElement designContainer;

    [MenuItem("Edixor/Tabs/Hot Keys")]
    public static void ShowTab()
    {
        ShowTab<HotKeyTab>();
    }

    private void Awake()
    {
        tabName = "Hot Keys";
        LoadUxml("auto");
        LoadUss("auto");

        OnHotKeyAdded += TabProcessing;
    }

    public void OnEnable()
    {
        ExDebug.Log("HotKeysTab OnEnable called.");
        RefreshHotKeysUI();
        root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void OnDisable()
    {
        ExDebug.Log("HotKeysTab OnDisable called.");
        OnHotKeyAdded -= TabProcessing;
        root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    protected void Start()
    {
        hotKeySettingEntries = container.ResolveNamed<HotKeySetting>(ServiceNames.HotKeySetting);
        hotkeyEntries = hotKeySettingEntries.GetAllEntries().ToList();

        designContainer = root.Q<VisualElement>("hotkeys-container");
        RefreshHotKeysUI();
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        var hotkeyHandler = container
            .ResolveNamed<IHotkeyCaptureHandler>(ServiceNames.IHotkeyCaptureHandler);
        if (hotkeyHandler.IsCapturing())
        {
            hotkeyHandler.Process(evt.imguiEvent);

            evt.StopPropagation();
        }
    }




    private void TabProcessing(HotKeyTabInfo info)
    {
        ExDebug.Log("Получено событие добавления горячей клавиши для вкладки: " + info.TabName);
        RefreshHotKeysUI();
    }

    private void RefreshHotKeysUI()
    {
        hotkeyEntries = hotKeySettingEntries.GetAllEntries().ToList();

        if (designContainer != null)
        {
            designContainer.Clear();

            // Добавим заголовки таблицы
            VisualElement headerRow = new VisualElement();
            headerRow.AddToClassList("hotkey-table-header");

            headerRow.Add(new Label("Index") { name = "column-index" });
            headerRow.Add(new Label("Key") { name = "column-key" });
            headerRow.Add(new Label("Name") { name = "column-name" });
            headerRow.Add(new Label("Combination") { name = "column-combo" });
            headerRow.Add(new Label("Options") { name = "column-options" });

            designContainer.Add(headerRow);

            int globalIndex = 1;
            foreach (var entry in hotkeyEntries)
            {
                if (entry.Values != null)
                {
                    for (int i = 0; i < entry.Values.Length; i++)
                    {
                        designContainer.Add(CreateTableRow(entry, i, globalIndex));
                        globalIndex++;
                    }
                }
            }
        }
    }

    private VisualElement CreateTableRow(HotKeySettingsAsset.KeyActionDictionaryEntry entry, int dataIndex, int globalIndex)
    {
        var keyData = entry.Values[dataIndex];

        VisualElement row = new VisualElement();
        row.AddToClassList("hotkey-table-row");

        row.Add(new Label(globalIndex.ToString()) { name = "column-index" });
        row.Add(new Label(entry.Key) { name = "column-key" });
        row.Add(new Label(keyData.Name) { name = "column-name" });

        var comboText = string.Join(" + ", keyData.Combination);
        row.Add(new Label(comboText) { name = "column-combo" });

        // Options buttons
        VisualElement optionsContainer = new VisualElement();
        optionsContainer.AddToClassList("options-container");

        Button editButton = new Button(() => EditHotKey(entry, dataIndex, null)) { text = "Edit" };
        editButton.AddToClassList("edit-button");

        Button toggleButton = new Button(() => ToggleHotKeyItem(entry, dataIndex, toggleButton: null))
        {
            text = keyData.enable ? "Disable" : "Enable"
        };
        toggleButton.AddToClassList("toggle-button");

        optionsContainer.Add(editButton);
        optionsContainer.Add(toggleButton);
        row.Add(optionsContainer);

        return row;
    }


    private void EditHotKey(HotKeySettingsAsset.KeyActionDictionaryEntry entry, int dataIndex, Label liveLabel)
    {
        ExDebug.Log($"Редактирование горячей клавиши для ключа: {entry.Key}, индекс: {dataIndex}");

        var hotkeyHandler = container.ResolveNamed<IHotkeyCaptureHandler>(ServiceNames.IHotkeyCaptureHandler);
        hotkeyHandler.OnCaptureChanged = (keys) =>
        {
            if (liveLabel != null)
            {
                liveLabel.text = keys.Count > 0
                    ? "Текущая комбинация: " + string.Join(" + ", keys)
                    : "Введите комбинацию...";
            }
        };

        if (liveLabel != null)
            liveLabel.text = "Введите комбинацию...";

        hotkeyHandler.StartCapture((keys) =>
        {
            if (liveLabel != null)
                liveLabel.text = "";

            entry.Values[dataIndex].Combination = new List<KeyCode>(keys);
            ExDebug.Log($"Горячая клавиша обновлена: {string.Join(" + ", keys)}");

            hotKeySettingEntries.UpdateEntry(entry.Key, entry);
            RefreshHotKeysUI();
        });
    }

    private void ToggleHotKeyItem(HotKeySettingsAsset.KeyActionDictionaryEntry entry, int dataIndex, Button toggleButton)
    {
        KeyActionData keyData = entry.Values[dataIndex];
        keyData.enable = !keyData.enable;
        ExDebug.Log($"Горячая клавиша для {entry.Key}, item {dataIndex + 1} теперь {(keyData.enable ? "Включена" : "Выключена")}");
        hotKeySettingEntries.UpdateEntry(entry.Key, entry);
        if (toggleButton != null)
            toggleButton.text = keyData.enable ? "Disable" : "Enable";
        RefreshHotKeysUI();
    }

    private void ToggleGroup(HotKeySettingsAsset.KeyActionDictionaryEntry entry, Button groupToggleButton)
    {
        foreach (var keyData in entry.Values)
        {
            keyData.enable = !keyData.enable;
        }
        ExDebug.Log($"Группа '{entry.Key}' теперь {(entry.Values[0].enable ? "Включена" : "Выключена")}");
        hotKeySettingEntries.UpdateEntry(entry.Key, entry);
        RefreshHotKeysUI();
    }

    private void DeleteGroup(string key)
    {
        ExDebug.Log($"Удаляем группу '{key}'");
        hotKeySettingEntries.RemoveEntry(key);
        RefreshHotKeysUI();
    }
}
