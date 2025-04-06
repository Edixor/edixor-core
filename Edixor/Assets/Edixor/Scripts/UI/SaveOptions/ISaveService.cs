using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

// Интерфейс для базовых сохранений (хранит список данных)
public interface ISaveAsset<TData>
{
    List<TData> SaveItems { get; set; }
}

// Интерфейс для сохранения текущего выбранного элемента
public interface ISaveCurrentAsset<TData> : ISaveAsset<TData>
{
    int CurrentIndex { get; set; }
}


// Интерфейс для сервисов сохранений
public interface ISaveService
{
    void Save();
    void Load();
}

// Базовый класс для всех настроек (работает с ScriptableObject)
public abstract class EdixorSetting<T> : ISaveService where T : ScriptableObject
{
    protected T settings;
    private string assetPath;

    protected EdixorSetting(string path)
    {
        assetPath = path;
        Load();
    }

    public virtual void Load()
    {
        settings = AssetDatabase.LoadAssetAtPath<T>(assetPath);
        if (settings == null)
        {
            settings = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
        }
    }

    public virtual void Save()
    {
        if (settings != null)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }

    public T GetSettings() => settings;
}

public abstract class EdixorCurrentSetting<TAsset, TData> : EdixorSetting<TAsset> 
    where TAsset : ScriptableObject, ISaveCurrentAsset<TData>
{
    private readonly IRegister _register;

    protected EdixorCurrentSetting(string path, IRegister register) : base(path) 
    {
        _register = register ?? throw new ArgumentNullException(nameof(register));
    }

    public TData GetCurrentItem()
    {
        var settings = GetSettings();
        if (settings.SaveItems != null && settings.CurrentIndex >= 0 && settings.CurrentIndex < settings.SaveItems.Count)
        {
            var item = settings.SaveItems[settings.CurrentIndex];
            if (item == null)
            {
                string typeName = typeof(TData).Name;
                Type resolvedType = _register.RegisterType(typeName);
                if (resolvedType != null)
                {
                    item = (TData)Activator.CreateInstance(resolvedType);
                    settings.SaveItems[settings.CurrentIndex] = item;
                }
            }
            return item;
        }
        return default;
    }

    public void SetCurrentItem(int index)
    {
        var settings = GetSettings();
        if (index >= 0 && index < settings.SaveItems.Count)
        {
            settings.CurrentIndex = index;
            Save();
        }
    }
}
