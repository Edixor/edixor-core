using System.Collections.Generic;
using UnityEngine;
using ExTools;
using System;

public class KeyAction
{
    public KeyActionData Data { get; set; }
    public ExHotKey Logic { get; set; }

    public KeyAction(KeyActionData keyActionData, ExHotKey ExHotKey, DIContainer container)
    {
        if (keyActionData == null)
        {
            ExDebug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }

        this.Data = keyActionData;

        if (ExHotKey == null)
        {
            ExHotKey logic = new ExHotKey(keyActionData.ScriptLogic, container);
            Logic = logic.GetLogic();
        }
    }

    public KeyAction(KeyActionData keyActionData, DIContainer container)
    {
        if (keyActionData == null)
        {
            ExDebug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }

        Data = keyActionData;
        
        if (Logic == null)
        {
            ExHotKey logic = new ExHotKey(keyActionData.ScriptLogic, container);
            Logic = logic.GetLogic();
        }
    }

    public void Activate()
    {
        if (Logic != null)
        {
            Logic.Activate();
        }
        else
        {
            ExDebug.LogError("ExHotKey is null in container: " + Data.Name);
        }
    }
}
