using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyAction
{
    public KeyActionData keyActionData { get; set; }
    public KeyActionLogic keyActionLogic { get; set; }

    public KeyAction(KeyActionData keyActionData, KeyActionLogic keyActionLogic)
    {
        if (keyActionData == null)
        {
            Debug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }
        this.keyActionData = keyActionData;
        if (keyActionLogic == null)
        {
            Debug.LogError("KeyActionLogic is null in KeyAction constructor.");
            return;
        }
        this.keyActionLogic = keyActionLogic;
    }

    public KeyAction(KeyActionData keyActionData, DIContainer container)
    {
        if (keyActionData == null)
        {
            Debug.LogError("KeyActionData is null in KeyAction constructor.");
            return;
        }
        this.keyActionData = keyActionData;
        keyActionLogic = container.ResolveNamed<KeyActionLogic>(keyActionData.LogicKey);
        if (keyActionLogic == null)
        {
            Debug.LogError($"KeyActionLogic not found for {keyActionData.LogicKey} in KeyAction constructor.");
            return;
        }
    }

    public void Execute()
    {
        if (keyActionLogic != null)
        {
            keyActionLogic.Execute();
        }
        else
        {
            Debug.LogError("KeyActionLogic is null in container: " + keyActionData.Name);
        }
    }
}
