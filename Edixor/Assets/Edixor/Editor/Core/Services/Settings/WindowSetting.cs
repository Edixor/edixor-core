using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ExTools;

public class WindowStateSetting : EdixorSettingData<WindowStateSettingsAsset>
{
    public WindowStateSetting(string nameFolder)
        : base($"WindowStates/{nameFolder}", "WindowStateSettings.asset")
    { }

    public bool IsWindowOpen() => Settings.isWindowOpen;
    public void SetWindowOpen(bool open)
    {
        Settings.isWindowOpen = open;
        SaveSettings();
    }
    public Rect GetOriginalWindowRect() => Settings.originalWindowRect;
    public void SetOriginalWindowRect(Rect r)
    {
        Settings.originalWindowRect = r;
        EditorUtility.SetDirty(Settings);
        AssetDatabase.SaveAssets();
        ExDebug.Log($"SetOriginalWindowRect: {r}");
    }
    public bool GetMinimized() => Settings.isMinimized;
    public void SetMinimized(bool m)
    {
        Settings.isMinimized = m;
        SaveSettings();
    }
    public VisualElement GetRootElement() => Settings.rootElement;
    public void SetRootElement(VisualElement e) => Settings.rootElement = e;

    public string GetString(string key)
    {
        if (Settings.stringSettings == null)
            Settings.stringSettings = new List<WindowStateSettingsAsset.StringEntry>();

        foreach (var entry in Settings.stringSettings)
        {
            if (entry.key == key)
                return entry.value;
        }
        return null;
    }

    public void SetString(string key, string value)
    {
        if (Settings.stringSettings == null)
            Settings.stringSettings = new List<WindowStateSettingsAsset.StringEntry>();

        for (int i = 0; i < Settings.stringSettings.Count; i++)
        {
            if (Settings.stringSettings[i].key == key)
            {
                Settings.stringSettings[i] = new WindowStateSettingsAsset.StringEntry { key = key, value = value };
                SaveSettings();
                return;
            }
        }
        Settings.stringSettings.Add(new WindowStateSettingsAsset.StringEntry { key = key, value = value });
        SaveSettings();
    }
}
