using System;
using UnityEngine;

public class KeyRestart : KeyActionLogic
{
    private IRestartable window;

    protected override void Init()
    {
        window = container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow);
        action = () =>
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
    }
}
