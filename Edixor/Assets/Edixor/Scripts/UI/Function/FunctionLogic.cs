using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class FunctionLogic
{
    protected DIContainer container;

    protected FunctionLogic(DIContainer container = null)
    {
        this.container = container;
    }

    public bool Empty() {
        return container == null;
    }

    public void SetContainer(DIContainer container) {
        this.container = container;
    }

    public abstract void Activate();
    public abstract void Init();
}