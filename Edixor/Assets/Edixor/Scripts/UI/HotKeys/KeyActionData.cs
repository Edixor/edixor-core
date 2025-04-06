using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyActionData", menuName = "Edixor/HK", order = 1)]
public class KeyActionData : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField]
    private string _actionName = "Default Action Name";

    public string Name
    {
        get { return _actionName; }
        set { _actionName = value; }
    }

    [Header("Setting Options")]
    [SerializeField]
    private bool _enable = true;
    public bool enable
    {
        get => _enable;
        set => _enable = value;
    }

    [SerializeField]
    private List<KeyCode> _combination = new List<KeyCode>();
    public List<KeyCode> Combination
    {
        get => _combination;
        set => _combination = value;
    }

    [TextArea] 
    public string Logica;
}