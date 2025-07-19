using UnityEditor;
using UnityEngine;
using System;
using ExTools;

public class EdixorSettingData<TData> where TData : ScriptableObject
{
    private TData _settings;
    private readonly string _virtualFolder;
    private readonly string _assetFileName;

    public EdixorSettingData(string virtualFolder, string assetFileName = null)
    {
        if (string.IsNullOrEmpty(virtualFolder))
            throw new ArgumentException("virtualFolder cannot be empty");

        _virtualFolder = virtualFolder;
        _assetFileName = assetFileName ?? $"{typeof(TData).Name}.asset";
    }

    protected TData Settings
    {
        get
        {
            if (_settings == null)
            {
                AssetDatabase.Refresh();
                _settings = LoadOrCreate();
            }
            return _settings;
        }
    }

    protected void SaveSettings()
    {
        if (_settings == null) return;
        EditorUtility.SetDirty(_settings);
        AssetDatabase.SaveAssets();
    }

    private TData LoadOrCreate()
    {
        string settingName = typeof(TData).Name;
        string folderPath = EdixorObjectLocator.Resolve(_virtualFolder);
        string assetPath = $"{folderPath}/{_assetFileName}";

        if (string.IsNullOrEmpty(folderPath))
        {
            ExDebug.LogError($"[{settingName}] Failed to resolve path '{_virtualFolder}'");
            return null;
        }

        var asset = AssetDatabase.LoadAssetAtPath<TData>(assetPath);
        if (asset == null)
        {
            ExDebug.LogWarning($"[{settingName}] Asset not found, creating new one.");
            asset = SOFactory.CreateSO<TData>(assetPath);
        }

        return asset;
    }

    public void ResetConfiguration()
    {
        if (_settings == null) return;

        if (_settings is IDefaultSeedable seedable)
        {
            seedable.EnsureDefaults();
        }
        else
        {
            var assetPath = AssetDatabase.GetAssetPath(_settings);
            ScriptableObject.DestroyImmediate(_settings, true);
            _settings = SOFactory.CreateSO<TData>(assetPath);
        }

        EditorUtility.SetDirty(_settings);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ExDebug.Log($"[EdixorSettingData] Configuration reset for {_settings.name}");
    }

    public void InvalidateCache()
    {
        _settings = null;
    }
}
