using UnityEngine;
using UnityEditor;

public class EdixorSetting<T> where T : ScriptableObject
{
    private T _settings;
    private string _path;

    public EdixorSetting(string path)
    {
        _path = path;
        
    }

    protected T Settings
    {
        get
        {
            if (_settings == null)
            {
                _settings = Resources.Load<T>(_path);
                if (_settings == null)
                {
                    _settings = ScriptableObject.CreateInstance<T>();
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
