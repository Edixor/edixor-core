using System.Collections.Generic;
using ExTools;
using UnityEngine;

public class TabSetting : EdixorSettingData<TabSettingsAsset>
{
    public string EdixorId { get; }

    public TabSetting(string edixorId)
        : base($"WindowStates/{edixorId}", "TabSettings.asset")
    {
        EdixorId = edixorId;
    }

    public List<TabData> GetTabData() =>
        Settings.Items;

    public int GetActiveIndex() =>
        Settings.CurrentIndex;

    public void SetActiveIndex(int idx)
    {
        Settings.CurrentIndex = idx;
        SaveSettings();
    }

    public void SetTabData(List<TabData> list)
    {
        Settings.Items = list;
        SaveSettings();
    }
}
