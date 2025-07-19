using System;

using ExTools.Controllers;
using ExTools.Settings;
using ExTools.HotKeys;

public interface IFactoryHotKey
{

    void InitializeData(HotKeySetting settingsData, HotKeyController controller);
    void CreateFromAssets(string path, HotKeyId key, Action action = null);
    void CreateExample(HotKeyExampleData data, HotKeyId key);
}
