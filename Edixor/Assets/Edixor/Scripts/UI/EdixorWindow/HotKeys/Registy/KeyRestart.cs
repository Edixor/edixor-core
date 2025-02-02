using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyRestart : KeyAction
{
    public override List<KeyCode> Combination => new List<KeyCode> { KeyCode.LeftShift, KeyCode.R };
    
    public override Action action => () => window.RestartWindow();

    public override string Name => "Restart";

    public KeyRestart(EdixorWindow window) : base(window) {
    }
}
