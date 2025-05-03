using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FactoryFunction : EdixorFactory<
    FunctionSettingsAsset, 
    FunctionSetting, 
    FunctionData, 
    string,                             
    FunctionExampleData                    
>, IFactoryFunction
{
    public FactoryFunction(FunctionSetting settingsData) : base(settingsData) { }

    public override void CreateExample(FunctionExampleData data, string key)
    {
  
    }

}
