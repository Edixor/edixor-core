using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using ExTools;
using System;


public class EdixorFactory<TAsset, TSettings, TItem, TKey, TData>
    where TItem : ScriptableObject
    where TAsset : ScriptableObject
    where TSettings : EdixorSettingsData<TAsset, TItem, TKey>
{
    private readonly TSettings _settingsData;

    public EdixorFactory(TSettings settingsData)
    {
        _settingsData = settingsData;
    }

    public virtual void CreateFromAssets(string path, TKey key, Action action = null)
    {
        ExDebug.Log($"Creating from assets: {path}");
        TItem asset = EdixorObjectLocator.LoadObject<TItem>(path);
        if (asset != null)
        {
            ExDebug.Log($"Loaded asset: {path}");
            SettingAddItem(asset, key);
        }
    }

    public virtual void CreateExample(TData data, TKey key)
    {
        TItem instance = ScriptableObject.CreateInstance<TItem>();
        SettingAddItem(instance, key);
    }

    protected virtual void SettingAddItem(TItem item, TKey key) => _settingsData.AddItem(item, key);
}
