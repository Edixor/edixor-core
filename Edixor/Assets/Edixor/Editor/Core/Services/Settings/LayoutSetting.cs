using LayoutSettingBase = EdixorSettingsData<LayoutSettingsAsset, LayoutData, string>;
using SettingCorrectItem = ISettingCorrectItem<LayoutData>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LayoutSetting : LayoutSettingBase, SettingCorrectItem
{
    public LayoutSetting()
        : base("SettingAsset", "LayoutSettings.asset")
    { }

    public override void AddItem(LayoutData item, string key)
    {
        if (item == null) return;
        if (string.IsNullOrEmpty(key)) return;
        if (GetItem(key) != null) return;

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

    public override void UpdateItem(string key, LayoutData item)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (item == null) return;

        var existingItem = GetItem(key);
        if (existingItem == null) return;

        int index = Settings.Items.IndexOf(existingItem);
        Settings.Items[index] = item;
        SaveSettings();
    }

    public override LayoutData GetItem(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        return Settings.Items.Find(x => x.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));
    }

    public override LayoutData[] GetAllItem()
    {
        return Settings.Items.ToArray();
    }

    public LayoutData GetCorrectItem()
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
