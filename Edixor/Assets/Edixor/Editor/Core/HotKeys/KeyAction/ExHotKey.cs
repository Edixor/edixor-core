using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ExTools;
using System;


[Serializable]
public class ExHotKey
{
    [NonSerialized]
    protected DIContainer container;
    private MonoScript script;

    public void SetContainer(DIContainer container)
    {
        this.container = container;
        if (script != null)
        {
            Init();
        }
    }

    public bool IsInitialized() => container != null;

    protected virtual void Init()
    {
        if (container == null)
        {
            ExDebug.LogError("Container is null in ExHotKey.Init");
            return;
        }

        if (script == null)
        {
            ExDebug.LogError("Script is null in ExHotKey.Init");
            return;
        }

        var logic = GetLogic();
        if (logic != null)
        {
            logic.SetContainer(container);
        }
    }

    public bool Empty() {
        return container == null;
    }

    public virtual void Activate()
    {

    }

    public ExHotKey GetLogic()
    {
        if (script == null)
            return null;

        var type = script.GetClass();
        if (type == null || !typeof(ExHotKey).IsAssignableFrom(type))
            return null;

        var instance = Activator.CreateInstance(type) as ExHotKey;
        if (instance != null)
        {
            instance.SetContainer(container);
        }
        return instance;
    }

    public void SetScript(MonoScript script)
    {
        this.script = script;
        if (container != null)
        {
            Init();
        }
    }

    public ExHotKey() { }

    public ExHotKey(MonoScript script, DIContainer container)
    {
        this.script = script;
        this.container = container;

        Init();
    }
}
