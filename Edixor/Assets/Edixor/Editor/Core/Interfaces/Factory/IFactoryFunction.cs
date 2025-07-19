using System;

using ExTools.Controllers;
using ExTools.Settings;
using ExTools.Functions;
public interface IFactoryFunction
{
    void InitializeData(FunctionSetting settingsData, FunctionController controller);
    void CreateExample(FunctionExampleData data, string key);
    void CreateFromAssets(string path, string key = null, Action action = null);
}