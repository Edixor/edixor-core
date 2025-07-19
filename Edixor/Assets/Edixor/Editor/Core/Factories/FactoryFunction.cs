using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using ExTools.Controllers;
using ExTools.Settings;

public class FactoryFunction : EdixorFactory<
    FunctionSettingsAsset,
    FunctionSetting,
    FunctionController,
    FunctionData,
    string,
    FunctionExampleData,
    Function
>, IFactoryFunction
{
    public FactoryFunction(FunctionSetting settingsData = null, FunctionController controller = null) : base(settingsData, controller) { }

    public void InitializeData(FunctionSetting settingsData, FunctionController controller)
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

    public override void CreateExample(FunctionExampleData data, string key)
    {

    }

}
