using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EdixorWindowSetting
{
    private readonly EdixorWindow window;
    private EdixorSettingSave settings;

    private const string SettingsPath = "Assets/Edixor/Scripts/UI/EdixorWindow/EdixorSettings.asset";
    private bool isInitialized = false;

    public EdixorWindowSetting(EdixorWindow context)
    {
        this.window = context;
        Load();
    }

    private void EnsureInitialized()
    {
        if (!isInitialized)
        {
            InitializeData();
            isInitialized = true;
        }
    }

    private void InitializeData()
    {
        // Регистрация дизайнов и первоначальная инициализация...
        EdixorDesignFactory designFactory = new EdixorDesignFactory();
        designFactory.RegisterAll(window);
        List<EdixorDesign> factoryDesigns = designFactory.GetAllItems();

        if (!settings.isModified)
        {
            EdixorFunctionFactory functionFactory = new EdixorFunctionFactory();
            functionFactory.RegisterAll(window);
            List<EdixorFunction> factoryFunctions = functionFactory.GetAllItems();

            KeyActionFactory hotKeysFactory = new KeyActionFactory();
            hotKeysFactory.RegisterAll(window);
            List<KeyAction> factoryHotKeys = hotKeysFactory.GetAllItems();

            settings.designs = factoryDesigns;
            settings.functions = factoryFunctions;
            settings.hotKeys = factoryHotKeys;
            settings.isModified = true;
        }
        else
        {
            settings.designs = factoryDesigns;
        }
        Save();
    }

    // Новый метод для сохранения списка открытых вкладок
    public void SetTabs(List<EdixorTab> newTabs)
    {
        settings.tabs = newTabs;
        Save();
    }

    public List<EdixorTab> GetTabs()
    {
        return settings.tabs;
    }

    // Методы для сохранения и получения индекса активной вкладки
    public void SetLastActiveTabIndex(int index)
    {
        settings.lastActiveTabIndex = index;
        Save();
    }

    public int GetLastActiveTabIndex()
    {
        return settings.lastActiveTabIndex;
    }

    // Остальные методы настроек остаются без изменений...
    public List<EdixorFunction> GetFunctions()
    {
        EnsureInitialized();
        return new List<EdixorFunction>(settings.functions);
    }

    public List<EdixorDesign> GetDesigns()
    {
        EnsureInitialized();
        Debug.Log("GetDesigns called with " + settings.designs.Count + " designs.");
        return new List<EdixorDesign>(settings.designs);
    }

    public List<KeyAction> GetHotKeys()
    {
        EnsureInitialized();
        Debug.Log("GetHotKeys called with " + settings.hotKeys.Count + " hotkeys.");
        return new List<KeyAction>(settings.hotKeys);
    }

    public void SetHotKeys(KeyAction keyAction, int index)
    {
        EnsureInitialized();
        if (index >= 0 && index < settings.hotKeys.Count)
        {
            settings.hotKeys[index] = keyAction;
            Save();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid hotkey index.");
        }
    }

    public EdixorDesign GetCurrentDesign(int index = -1)
    {
        EnsureInitialized();
        if (index >= 0 && index < settings.designs.Count)
        {
            return settings.designs[index];
        }

        if (settings.designIndex >= 0 && settings.designIndex < settings.designs.Count)
        {
            return settings.designs[settings.designIndex];
        }

        throw new IndexOutOfRangeException("Design index is out of range.");
    }

    public int GetDesignIndex()
    {
        EnsureInitialized();
        return settings.designIndex;
    }

    public int GetDesignVersion()
    {
        EnsureInitialized();
        return settings.designVersionIndex;
    }

    public void SetDesignIndex(int index)
    {
        EnsureInitialized();

        if (index >= 0 && index < settings.designs.Count)
        {
            settings.designIndex = index;
            Save();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid design index.");
        }
    }

    public void SetDesignIndex(int index, int versionIndex)
    {
        EnsureInitialized();

        if (index < 0 || index >= settings.designs.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid design index.");
        }

        settings.designIndex = index;
        settings.designVersionIndex = versionIndex;
        Save();
    }

    public void Load()
    {
        settings = AssetDatabase.LoadAssetAtPath<EdixorSettingSave>(SettingsPath);
        if (settings == null)
        {
            Debug.LogWarning("Settings not found, creating new default settings.");
            settings = ScriptableObject.CreateInstance<EdixorSettingSave>();
            AssetDatabase.CreateAsset(settings, SettingsPath);
            AssetDatabase.SaveAssets();
        }
    }

    public void Save()
    {
        if (settings != null)
        {
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }
    }

    public bool IsWindowOpen()
    {
        return settings.isWindowOpen;
    }

    public void SetWindowOpen(bool open)
    {
        settings.isWindowOpen = open;
        Save();
    }
}
