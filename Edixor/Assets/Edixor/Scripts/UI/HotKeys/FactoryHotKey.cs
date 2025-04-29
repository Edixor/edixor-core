using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FactoryHotKey : EdixorFactory<
    HotKeySaveAsset, 
    HotKeySetting, 
    KeyActionData, 
    HotKeyId,                             
    HotKeyExampleData                     
>, IFactoryHotKey
{
    public FactoryHotKey(HotKeySetting settingsData) : base(settingsData) { }

    public override void CreateExample(HotKeyExampleData data, HotKeyId key)
    {
        var instance = ScriptableObject.CreateInstance<KeyActionData>();
        if (instance != null)
        {
            instance.Name = data.name;
            instance.enable = true;
            instance.Combination = new List<KeyCode>(data.combination);

            typeof(KeyActionData)
                .GetProperty("LogicKey", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(instance, data.logicKey);

            string dir = "Assets/EdixorGenerated";
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            string path = AssetDatabase.GenerateUniqueAssetPath($"{dir}/{instance.Name}_{key}.asset");
            AssetDatabase.CreateAsset(instance, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            SettingAddItem(instance, key);
        }
    }
}
