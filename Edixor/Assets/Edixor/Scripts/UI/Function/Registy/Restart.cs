using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[Serializable]
public class Restart : FunctionLogica, IFunctionSetting
{
    private IRestartable window;
    [Header("Settings options")]
    [SerializeField]
    private bool _tabCleaning;
    public bool TabCleaning
    {
        get { return _tabCleaning; }
        set { _tabCleaning = value; }
    }

    [SerializeField]
    private bool _clearCache;
    public bool ClearCache
    {
        get { return _clearCache; }
        set { _clearCache = value; }
    }

    public Restart(DIContainer container) : base(container)
    {
        window = container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow);
    }

    public override void Init()
    {
        window = container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow);
    }

    public Restart() : base()
    {
    }

    public override void Activate()
    {
        if (window != null)
        {
            window.RestartWindow();
        }
        else
        {
            Debug.LogError("Window is null in Restart action");
        }
    }

    public void Setting(VisualElement root)
    {
        Toggle tabCleaningToggle = new Toggle("Tab cleaning");

        tabCleaningToggle.RegisterValueChangedCallback(evt =>
        {
            TabCleaning = evt.newValue; 
        });

        Toggle clearCacheToggle = new Toggle("Clear cache");
        
        clearCacheToggle.RegisterValueChangedCallback(evt =>
        {
            ClearCache = evt.newValue;
        });

        root.Add(tabCleaningToggle);
        root.Add(clearCacheToggle);
    }

}

