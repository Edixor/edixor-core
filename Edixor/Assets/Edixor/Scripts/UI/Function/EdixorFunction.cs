using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class EdixorFunction
{
    public abstract Texture2D Icon { get; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    protected EdixorWindow Window;

    protected EdixorFunction(EdixorWindow window)
    {
        this.Window = window;
    }

    public abstract void Activate();
}