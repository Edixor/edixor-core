using System;
using UnityEngine;

public class CustomKeyAction : KeyActionLogic
{
    private Action customAction;

    public CustomKeyAction(DIContainer container, Action customAction = null) : base(container)
    {
        this.customAction = customAction;
        Init();
    }

    protected override void Init()
    {
        if (customAction != null)
        {
            action = customAction;
        }
        else
        {
            action = () => { };
        }
    }
}
