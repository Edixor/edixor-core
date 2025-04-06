using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class KeyActionLogica
{
    [NonSerialized]
    protected DIContainer container;

    public void SetContainer(DIContainer container)
    {
        this.container = container;
        Init();
    }

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

    public virtual Action Action { get; }

    protected KeyActionLogica() { }

    protected KeyActionLogica(DIContainer container)
    {
        this.container = container;
    }
}
