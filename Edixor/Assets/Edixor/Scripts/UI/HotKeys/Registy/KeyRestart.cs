using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyRestart : KeyActionLogica
{
    private IRestartable window;
    public override Action Action => () =>
    {
        if (window != null)
        {
            window.RestartWindow();
        }
        else
        {
            Debug.LogError("Window is null in Restart action");
        }
    };

    protected override void Init() { 
        window = container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow);
    }
}
