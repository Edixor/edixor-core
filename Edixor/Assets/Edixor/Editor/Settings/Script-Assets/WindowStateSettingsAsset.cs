using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "WindowStateSettings", menuName = "Edixor/Settings/WindowSettings", order = 1)]
public class WindowStateSettingsAsset : ScriptableObject
{
    public Rect originalWindowRect;
    public bool isWindowOpen;
    public bool isMinimized;
    public bool isMaximized;
    public VisualElement rootElement;

    [Serializable]
    public struct StringEntry
    {
        public string key;
        public string value;
    }

    public List<StringEntry> stringSettings = new List<StringEntry>();

    public string GetString(string key)
    {
        for (int i = 0; i < stringSettings.Count; i++)
        {
            if (stringSettings[i].key == key)
                return stringSettings[i].value;
        }
        return null;
    }

    public void SetString(string key, string value)
    {
        for (int i = 0; i < stringSettings.Count; i++)
        {
            if (stringSettings[i].key == key)
            {
                var entry = stringSettings[i];
                entry.value = value;
                stringSettings[i] = entry;
                return;
            }
        }
        stringSettings.Add(new StringEntry { key = key, value = value });
    }
}
