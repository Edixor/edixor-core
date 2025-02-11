using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyRestart : KeyAction
{
    [SerializeField]
    private List<KeyCode> combination = new List<KeyCode> { KeyCode.LeftShift, KeyCode.R };

    public override List<KeyCode> Combination => combination;

    public override Action Action => () =>
    {
        if (window != null)
        {
            window.RestartWindow();
        }
        else
        {
            Debug.LogError("Window is null in Restart action");
        }
    };

    public KeyRestart() : base() { 
        Name = "Restart";
    }
    public KeyRestart(EdixorWindow window) : base(window) { 
        Name = "Restart";
    }
}
