using System;
using System.Collections.Generic;
using UnityEngine;

public class Exit : KeyAction
{
    public override List<KeyCode> Combination => new List<KeyCode> { KeyCode.LeftShift, KeyCode.E };
    
    public override Action action => () => window.Close();

    public override string Name => "Exit";

    public Exit(EdixorWindow window) : base(window) {
    }
}
