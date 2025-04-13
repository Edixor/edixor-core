using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class KeyActionLogic
{
    [NonSerialized]
    protected DIContainer container;

    protected Action Action;

    public void SetContainer(DIContainer container)
    {
        this.container = container;
        Init();
    }

    public bool IsInitialized() => container != null;

    protected abstract void Init();

    public virtual void Execute()
    {
        if (container == null)
        {
            Debug.LogError("container is null in action: ");
            return;
        }
        Action?.Invoke();
    }

    public Action action
    {
        get => Action;
        set => Action = value;
    }

    public KeyActionLogic() { }

    public KeyActionLogic(DIContainer container)
    {
        this.container = container;
        Init();
    }
}
