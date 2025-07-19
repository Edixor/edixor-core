using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using ExTools;
using System;


public class EdixorFactory<TAsset, TSettings, TController, TItem, TKey, TData, TDataFull>
    where TItem : ScriptableObject
    where TAsset : ScriptableObject
    where TSettings : EdixorSettingsData<TAsset, TItem, TKey>
    where TController : ListControllerBase<TDataFull>
{
    private TSettings _settingsData;
    private TController _controller;

    public EdixorFactory(TSettings settingsData, TController controller)
    {
        _settingsData = settingsData;
        _controller = controller;
    }

    public void Initialize(TSettings settingsData, TController controller)
    {
        if (settingsData == null)
        {
            ExDebug.LogError($"[EdixorFactory] Settings data is null");
            return;
        }

        _settingsData = settingsData;
        
        if (controller == null)
        {
            ExDebug.LogError($"[EdixorFactory] Controller is null");
            return;
        }
        
        _controller = controller;
    }

    public virtual void CreateFromAssets(string path, TKey key, Action action = null)
    {
        ExDebug.Log($"Creating from assets: {EdixorObjectLocator.Resolve(path)}");
        TItem asset = EdixorObjectLocator.LoadObject<TItem>(path);
        if (asset != null)
        {
            ExDebug.Log($"Loaded asset: {EdixorObjectLocator.Resolve(path)}");
            SettingAddItem(asset, key);
        }
    }

    public virtual void CreateExample(TData data, TKey key)
    {
        TItem instance = ScriptableObject.CreateInstance<TItem>();
        SettingAddItem(instance, key);
    }

    protected virtual void SettingAddItem(TItem item, TKey key)
    {
        _settingsData.AddItem(item, key);
        if (_settingsData is ISettingItemFull<TDataFull, TKey> settingItemFull)
        {
            _controller.AddItem(settingItemFull.GetItemFull(key));
        }
    } 
}
