using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyAction
{
    public KeyActionData keyActionData { get; set; }
    public KeyActionLogica KeyActionLogica { get; set; }

    public KeyAction(KeyActionData keyActionData, KeyActionLogica keyActionLogica)
    {
        this.keyActionData = keyActionData;
        this.KeyActionLogica = keyActionLogica;
    }

    public void Execute()
    {
        if (KeyActionLogica != null)
        {
            KeyActionLogica.Execute();
        }
        else
        {
            Debug.LogError("KeyActionLogica is null in container: " + keyActionData.Name);
        }
    }
}
