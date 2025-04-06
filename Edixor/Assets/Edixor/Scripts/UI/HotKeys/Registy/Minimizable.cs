using System;
using System.Collections.Generic;
using UnityEngine;

public class Minimizable : KeyActionLogica
{
    private IMinimizable window;
    public override Action Action => () =>
    {
        if (window != null)
        {
            window.MinimizeWindow();
        }
        else
        {
            Debug.LogError("Window is null in Close action");
        }
    };

    protected override void Init()
    {
        window = container.ResolveNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow);
    }
}