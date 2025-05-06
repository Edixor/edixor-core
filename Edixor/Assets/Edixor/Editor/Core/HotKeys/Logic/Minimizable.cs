using ExTools;
using System;

public class Minimizable : KeyActionLogic
{
    private IMinimizable window;

    protected override void Init()
    {
        window = container.ResolveNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow);
        action = () =>
        {
            if (window != null)
            {
                window.MinimizeWindow();
            }
            else
            {
                ExDebug.LogError("Window is null in Minimize action");
            }
        };
    }
}
