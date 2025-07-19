using HotKeySettingEntries = ISettingEntries<HotKeySettingsAsset.KeyActionDictionaryEntry, string>;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

using ExTools.Settings;
using ExTools;

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
        LoadIcon("Resources/Images/Icons/keys.png");
        OnHotKeyAdded += TabProcessing;
    }

    public void Start()
    {
        hotKeySettingEntries = container.ResolveNamed<HotKeySetting>(ServiceNameKeys.HotKeySetting);
        hotkeyEntries = hotKeySettingEntries.GetAllEntries().ToList();
        designContainer = root.Q<VisualElement>("hotkeys-container");
        RefreshHotKeysUI();
    }

    public void OnEnable()
    {
        if (container == null || root == null) return;
        ExDebug.Log("HotKeyTab OnEnable");
        if (hotKeySettingEntries == null)
            hotKeySettingEntries = container.ResolveNamed<HotKeySetting>(ServiceNameKeys.HotKeySetting);
        if (designContainer == null)
            designContainer = root.Q<VisualElement>("hotkeys-container");
        RefreshHotKeysUI();
        root.RegisterCallback<KeyDownEvent>(OnKeyDown);
    }

    public void OnDisable()
    {
        ExDebug.Log("HotKeyTab OnDisable");
        OnHotKeyAdded -= TabProcessing;
        if (root != null)
            root.UnregisterCallback<KeyDownEvent>(OnKeyDown);
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        var handler = container.ResolveNamed<IHotkeyCaptureHandler>(ServiceNameKeys.HotkeyCaptureHandler);
        if (handler.IsCapturing())
        {
            handler.Process(evt.imguiEvent);
            evt.StopPropagation();
        }
    }

    private void TabProcessing(HotKeyTabInfo info)
    {
        ExDebug.Log($"HotKeyTab event for: {info.TabName}");
        RefreshHotKeysUI();
    }

    private void RefreshHotKeysUI()
    {
        if (hotKeySettingEntries == null || designContainer == null) return;
        hotkeyEntries = hotKeySettingEntries.GetAllEntries().ToList();
        designContainer.Clear();

        var header = new VisualElement();
        header.AddToClassList("hotkey-table-header");
        header.Add(new Label("№") { name = "column-index" });
        header.Add(new Label("Base") { name = "column-key" });
        header.Add(new Label("Name") { name = "column-name" });
        header.Add(new Label("Combination") { name = "column-combo" });
        header.Add(new Label("Options") { name = "column-options" });
        designContainer.Add(header);

        int globalIndex = 1;
        foreach (var entry in hotkeyEntries)
        {
            if (entry.Values == null) continue;
            for (int i = 0; i < entry.Values.Length; i++)
            {
                designContainer.Add(CreateTableRow(entry, i, globalIndex++));
            }
        }
    }

    private VisualElement CreateTableRow(HotKeySettingsAsset.KeyActionDictionaryEntry entry, int dataIndex, int globalIndex)
    {
        var keyData = entry.Values[dataIndex];
        var row = new VisualElement();
        row.AddToClassList("hotkey-table-row");

        var idx = new Label(globalIndex.ToString()) { name = "column-index" };
        idx.AddToClassList("E-label");
        var baseKey = new Label(entry.Key) { name = "column-key" };
        baseKey.AddToClassList("E-label");
        var nameLabel = new Label(keyData.Name) { name = "column-name" };
        nameLabel.AddToClassList("E-label");
        row.Add(idx);
        row.Add(baseKey);
        row.Add(nameLabel);

        var comboText = string.Join(" + ", keyData.Combination);
        var comboLabel = new Label(comboText) { name = "column-combo" };
        comboLabel.AddToClassList("E-label");
        row.Add(comboLabel);

        var options = new VisualElement();
        options.AddToClassList("options-container");
        var enableToggle = new Toggle() { value = keyData.enable, name = "column-enable-toggle" };
        enableToggle.RegisterValueChangedCallback(evt =>
        {
            keyData.enable = evt.newValue;
            hotKeySettingEntries.UpdateEntry(entry.Key, entry);
            RefreshHotKeysUI();
        });
        var editButton = new Button(() => EditHotKey(entry, dataIndex, null)) { text = "Edit" };
        editButton.AddToClassList("edit-button");
        editButton.AddToClassList("E-button");
        options.Add(enableToggle);
        options.Add(editButton);

        row.Add(options);
        return row;
    }

    private void EditHotKey(HotKeySettingsAsset.KeyActionDictionaryEntry entry, int dataIndex, Label liveLabel)
    {
        ExDebug.Log($"Edit hotkey {entry.Key}[{dataIndex}]");
        var handler = container.ResolveNamed<IHotkeyCaptureHandler>(ServiceNameKeys.HotkeyCaptureHandler);
        handler.OnCaptureChanged = keys =>
        {
            if (liveLabel != null)
                liveLabel.text = keys.Count > 0
                    ? "Current: " + string.Join(" + ", keys)
                    : "Enter combination...";
        };
        if (liveLabel != null) liveLabel.text = "Enter combination...";
        handler.StartCapture(keys =>
        {
            if (liveLabel != null) liveLabel.text = "";
            entry.Values[dataIndex].Combination = new List<KeyCode>(keys);
            hotKeySettingEntries.UpdateEntry(entry.Key, entry);
            RefreshHotKeysUI();
        });
    }
}
