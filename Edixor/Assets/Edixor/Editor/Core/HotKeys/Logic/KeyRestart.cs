using ExTools;
using System;

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
                ExDebug.LogError("Window is null in Restart action");
            }
        };
    }
}
