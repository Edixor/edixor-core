using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HotKeyService : EdixorSetting<HotKeySaveAsset>
{
    public HotKeyService() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/HotKeySettings.asset")) { }

    public List<KeyActionData> GetHotKeys() => settings.SaveItems;

    public void AddHotKey(KeyActionData keyAction)
    {
        settings.SaveItems.Add(keyAction);
        Save();
    }
}
