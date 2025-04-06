using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public class Close : FunctionLogica, IFunctionSetting
{
    private IMinimizable window;
    public override void Activate()
    {
        if (window != null)
        {
            window.MinimizeWindow();
        }
        else
        {
            Debug.LogError("Window is null in Close action");
        }
    }

    public override void Init()
    {
        window = container.ResolveNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow);
    }

    public void Setting(VisualElement root)
    {
        
    }
}
