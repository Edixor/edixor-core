using System;
using UnityEngine;

public class Exit : KeyActionLogic
{
    private IClosable window;

    protected override void Init()
    {
        window = container.ResolveNamed<IClosable>(ServiceNames.IClosable_EdixorWindow);
        action = () =>
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
    }
}
