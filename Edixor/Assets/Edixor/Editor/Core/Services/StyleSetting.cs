using LayoutSettingBase = EdixorSettingsData<StyleSettingsAsset, StyleData, string>;
using SettingCorrectItem = ISettingCorrectItem<StyleData>;

using System.Collections.Generic;
using UnityEngine;

public class StyleSetting : LayoutSettingBase, SettingCorrectItem
{
    public StyleSetting()
        : base("SettingAsset/StyleSettings.asset")
    { }

    public override void AddItem(StyleData item, string key)
    {
        if (item == null || string.IsNullOrEmpty(key) || GetItem(key) != null)
            return;

        Settings.Items.Add(item);
        Settings.CurrentIndex = Settings.Items.Count - 1;
        SaveSettings();
    }

    public override void RemoveItem(string key)
    {
        if (string.IsNullOrEmpty(key)) return;

        var item = GetItem(key);
        if (item == null) return;

        Settings.Items.Remove(item);
        Settings.CurrentIndex = Mathf.Clamp(Settings.CurrentIndex - 1, 0, Settings.Items.Count - 1);
        SaveSettings();
    }

    public override void UpdateItem(string key, StyleData item)
    {
        if (string.IsNullOrEmpty(key) || item == null) return;

        var existingItem = GetItem(key);
        if (existingItem == null) return;

        int index = Settings.Items.IndexOf(existingItem);
        Settings.Items[index] = item;
        SaveSettings();
    }

    public override StyleData GetItem(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;

        return Settings.Items.Find(x => x.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));
    }

    public override StyleData[] GetAllItem()
    {
        return Settings.Items.ToArray();
    }

    public StyleData GetCorrectItem()
    {
        int idx = Settings.CurrentIndex;
        if (idx < 0 || idx >= Settings.Items.Count) return Settings.Items[0];
        return Settings.Items[idx];
    }

    public void UpdateIndex(int index)
    {
        if (index < 0 || index >= Settings.Items.Count) return;
        Settings.CurrentIndex = index;
        SaveSettings();
    }
}
