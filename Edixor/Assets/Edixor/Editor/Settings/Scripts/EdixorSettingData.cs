using UnityEngine;
using UnityEditor;
using ExTools;

public class EdixorSettingData<TData> where TData : ScriptableObject
{
    private TData _settings;
    private string _path;

    public EdixorSettingData(string path)
    {
        _path = path;
    }

    protected TData Settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = EdixorObjectLocator.LoadObject<TData>(_path);
                if (_settings == null)
                {
                    ExDebug.Log($"not path {_path}");
                    _settings = ScriptableObject.CreateInstance<TData>();
                    AssetDatabase.CreateAsset(_settings, _path);
                    AssetDatabase.SaveAssets();
                }
            }
            return _settings;
        }
    }

    protected void SaveSettings()
    {
        if (_settings != null)
        {
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }
}
