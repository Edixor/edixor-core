using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class EdixorWindowSetting
{
    private readonly EdixorWindow window;
    private EdixorSettings settings;

    private const string SettingsPath = "Assets/Edixor/Scripts/UI/EdixorWindow/EdixorSettings.asset";

    private List<EdixorDesign> designs;
    private List<EdixorFunction> functions;

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
        EdixorFunctionFactory functionFactory = new EdixorFunctionFactory();
        functionFactory.RegisterAllFunctions(window);

        functions = functionFactory.GetAllFunctions();

        designs = new List<EdixorDesign>
        {
            new StandardDesign(window),
            new IntertedDesign(window)
        };
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
        settings = AssetDatabase.LoadAssetAtPath<EdixorSettings>(SettingsPath);
        if (settings == null)
        {
            Debug.LogWarning("Settings not found, creating new default settings.");
            settings = ScriptableObject.CreateInstance<EdixorSettings>();
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
}
