using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FactoryHotKey : EdixorFactory<
    HotKeySettingsAsset, 
    HotKeySetting, 
    KeyActionData, 
    HotKeyId,                             
    HotKeyExampleData                     
>, IFactoryHotKey
{
    public FactoryHotKey(HotKeySetting settingsData) : base(settingsData) { }

    public override void CreateExample(HotKeyExampleData data, HotKeyId key)
    {
 
    }
}
