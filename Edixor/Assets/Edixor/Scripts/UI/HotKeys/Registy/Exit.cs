using System;
using System.Collections.Generic;
using UnityEngine;

public class Exit : KeyActionLogica
{
    private IClosable window;
    public override Action Action => () =>
    {
        if (window != null)
        {
            window.CloseWindow();
        }
        else
        {
            Debug.LogError("Window is null in Exit action");
        }
    };

    protected override void Init()
    {
        window = container.ResolveNamed<IClosable>(ServiceNames.IClosable_EdixorWindow);
    }
}
