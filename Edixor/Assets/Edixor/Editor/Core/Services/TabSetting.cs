using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TabSetting : EdixorSettingData<TabSettingsAsset>
{
    public TabSetting() : base("SettingAsset/TabSettings.asset") { }

    public List<EdixorTab> GetTabs() => Settings.Items;
    public int GetActiveTab() => Settings.CurrentIndex;
    public void SetLastActiveTabIndex(int index) {
        Settings.CurrentIndex = index;
        SaveSettings();
    }

    public void SetTabs(List<EdixorTab> newTabs)
    {
        Settings.Items = newTabs;
        SaveSettings();
    }

    public void AddTabs(string id, EdixorTab newTab)
    {
        Settings.Items.Add(newTab);
        SaveSettings();
    }
}
