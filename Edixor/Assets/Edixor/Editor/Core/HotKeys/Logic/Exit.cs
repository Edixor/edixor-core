using ExTools;
using System;
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
                ExDebug.LogError("Window is null in Exit action");
            }
        };
    }
}
