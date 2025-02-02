using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class KeyAction
{
    public abstract List<KeyCode> Combination { get; }

    public abstract string Name { get; }
    
    public virtual Action action { get; } 

    protected EdixorWindow window;

    public KeyAction(EdixorWindow window)
    {
        this.window = window;
    }

    public void Action()
    {
        action?.Invoke();
    }
}
