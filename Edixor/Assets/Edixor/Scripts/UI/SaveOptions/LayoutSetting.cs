using LayoutSettingBase = EdixorSettingsData<LayoutSaveAsset, LayoutData, string>;
using SettingCorrectItem = ISettingCorrectItem<LayoutData>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LayoutSetting : LayoutSettingBase, SettingCorrectItem
{
    public LayoutSetting() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorLayoutDataSettings.asset")) { }

    public override void AddItem(LayoutData item, string key)
    {
        if (item == null) return;
        if (string.IsNullOrEmpty(key)) return;
        if (GetItem(key) != null) return;

        Settings.SaveItems.Add(item);
        Settings.CurrentIndex = Settings.SaveItems.Count - 1;
        SaveSettings();
    }

    public override void RemoveItem(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        var item = GetItem(key);
        if (item == null) return;

        Settings.SaveItems.Remove(item);
        Settings.CurrentIndex = Mathf.Clamp(Settings.CurrentIndex - 1, 0, Settings.SaveItems.Count - 1);
        SaveSettings();
    }

    public override void UpdateItem(string key, LayoutData item)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (item == null) return;

        var existingItem = GetItem(key);
        if (existingItem == null) return;

        int index = Settings.SaveItems.IndexOf(existingItem);
        Settings.SaveItems[index] = item;
        SaveSettings();
    }

    public override LayoutData GetItem(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        return Settings.SaveItems.Find(x => x.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));
    }

    public override LayoutData[] GetAllItem()
    {
        return Settings.SaveItems.ToArray();
    }

    public LayoutData GetCorrectItem()
    {
        int idx = Settings.CurrentIndex;
        if (idx < 0 || idx >= Settings.SaveItems.Count) return Settings.SaveItems[0];
        return Settings.SaveItems[idx];
    }

    public void UpdateIndex(int index)
    {
        if (index < 0 || index >= Settings.SaveItems.Count) return;
        Settings.CurrentIndex = index;
        SaveSettings();
    }
}
