using System.Collections.Generic;
using UnityEngine;
using ExTools;
using System;

public class KeyAction
{
    public KeyActionData Data { get; set; }
    public KeyActionLogic Logic { get; set; }

    public KeyAction(KeyActionData keyActionData, KeyActionLogic keyActionLogic)
    {
        if (keyActionData == null)
        {
            ExDebug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }
        this.Data = keyActionData;
        if (keyActionLogic == null)
        {
            ExDebug.LogError("KeyActionLogic is null in KeyAction constructor.");
            return;
        }
        this.Logic = keyActionLogic;
    }

    public KeyAction(KeyActionData keyActionData, DIContainer container)
    {
        if (keyActionData == null)
        {
            ExDebug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }
        this.Data = keyActionData;
        Logic = container.ResolveNamed<KeyActionLogic>(keyActionData.LogicKey);
        if (Logic == null)
        {
            ExDebug.LogError($"KeyActionLogic not found for {keyActionData.LogicKey} in KeyAction constructor.");
            return;
        }
    }

    public void Execute()
    {
        if (Logic != null)
        {
            Logic.Execute();
        }
        else
        {
            ExDebug.LogError("KeyActionLogic is null in container: " + Data.Name);
        }
    }
}
