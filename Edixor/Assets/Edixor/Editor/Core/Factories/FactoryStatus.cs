using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FactoryStatus : EdixorFactory<
    StatusSettingsAsset,
    StatusSetting,
    StatusController,
    StatusData,
    string,
    string,
    Status             
>, IFactoryStatus
{
    public FactoryStatus(StatusSetting settingsData = null, StatusController controller = null) : base(settingsData, controller) { }

    public void InitializeData(StatusSetting settingsData, StatusController controller)
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

    public override void CreateExample(string data, string key)
    {
        
    }
}
