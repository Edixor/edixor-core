using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EdixorWindowSetting
{
    private readonly EdixorWindow window;
    private EdixorSettingSave settings;

    private const string SettingsPath = "Assets/Edixor/Scripts/UI/EdixorWindow/EdixorSettings.asset";

    private List<EdixorDesign> designs;
    private List<EdixorFunction> functions;
    private List<KeyAction> hotKeys;

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
        if (settings.isModified == false)
        {
            EdixorDesignFactory designFactory = new EdixorDesignFactory();
            designFactory.RegisterAll(window);
            designs = designFactory.GetAllItems();

            EdixorFunctionFactory functionFactory = new EdixorFunctionFactory();
            functionFactory.RegisterAll(window);
            functions = functionFactory.GetAllItems();

            KeyActionFactory hotKeysFactory = new KeyActionFactory();
            hotKeysFactory.RegisterAll(window);
            hotKeys = hotKeysFactory.GetAllItems();

            settings.designs = designs;
            settings.functions = functions;
            settings.hotKeys = hotKeys;

            settings.isModified = true;
            Save();
        } 
        else
        {
            EdixorDesignFactory designFactory = new EdixorDesignFactory();
            designFactory.RegisterAll(window);
            designs = designFactory.GetAllItems();

            hotKeys = settings.hotKeys;
            functions = settings.functions;

            Save();
        }
    }

    public List<EdixorFunction> GetFunctions()
    {
        EnsureInitialized();
        return new List<EdixorFunction>(functions);
    }

    public List<EdixorDesign> GetDesigns()
    {
        EnsureInitialized();
        return new List<EdixorDesign>(designs);
    }

    public List<KeyAction> GetHotKeys()
    {
        EnsureInitialized();
        return hotKeys;
    }

    public void SetHotKeys(KeyAction keyAction, int index)
    {
        EnsureInitialized();
        if (index >= 0 && index < hotKeys.Count)
        {
            hotKeys[index] = keyAction;
            Save();
        }
        else
        {
            throw new System.ArgumentOutOfRangeException(nameof(index), "Invalid hotkey index.");
        }
    }

    public EdixorDesign GetCurrentDesign(int index = -1)
    {
        EnsureInitialized();
        if (index >= 0 && index < designs.Count)
        {
            return designs[index];
        }

        if (settings.designIndex >= 0 && settings.designIndex < designs.Count)
        {
            return designs[settings.designIndex];
        }

        throw new System.IndexOutOfRangeException("Design index is out of range.");
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

        if (index >= 0 && index < designs.Count)
        {
            settings.designIndex = index;
            Save();
        }
        else
        {
            throw new System.ArgumentOutOfRangeException(nameof(index), "Invalid design index.");
        }
    }

    public void SetDesignIndex(int index, int versionIndex)
    {
        EnsureInitialized();

        if (index < 0 || index >= designs.Count)
        {
            throw new System.ArgumentOutOfRangeException(nameof(index), "Invalid design index.");
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

    // Новые методы для работы со статусом окна:

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
