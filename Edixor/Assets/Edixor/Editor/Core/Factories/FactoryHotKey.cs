using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using ExTools.Controllers;
using ExTools.Settings;
using ExTools.HotKeys;

public class FactoryHotKey : EdixorFactory<
    HotKeySettingsAsset,
    HotKeySetting,
    HotKeyController,
    KeyActionData,
    HotKeyId,
    HotKeyExampleData,
    KeyAction                 
>, IFactoryHotKey
{
    public FactoryHotKey(HotKeySetting settingsData = null, HotKeyController controller = null) : base(settingsData, controller) { }

    public void InitializeData(HotKeySetting settingsData, HotKeyController controller = null)
    {
        if (settingsData == null)
        {
            Debug.LogError($"[FactoryHotKey] Settings data is null");
            return;
        }
        if (controller == null)
        {
            Debug.LogError($"[FactoryHotKey] Controller is null");
            return;
        }
        base.Initialize(settingsData, controller);
    }

    public override void CreateExample(HotKeyExampleData data, HotKeyId key)
    {

    }
}
