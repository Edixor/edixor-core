using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TabService : EdixorCurrentSetting<TabSaveAsset, EdixorTab>
{
    public TabService(IRegister register) : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorTabSettings.asset"), register) { }

    public List<EdixorTab> GetTabs() => settings.SaveItems;
    public int GetActiveTab() => settings.CurrentIndex;
    public void SetLastActiveTabIndex(int index) {
        settings.CurrentIndex = index;
        Save();
    }

    public void SetTabs(List<EdixorTab> newTabs)
    {
        settings.SaveItems = newTabs;
        Save();
    }
}