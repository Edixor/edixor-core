using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "KeyActionData", menuName = "Edixor/HK", order = 1)]
public class KeyActionData : ScriptableObject
{
    [SerializeField]
    private string _actionName = "Default Action Name";

    public string Name
    {
        get => _actionName;
        set => _actionName = value;
    }

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

    [SerializeField]
    private string logicKey;
    public string LogicKey
    {
        get => logicKey;
        set => logicKey = value;
    }

    public override string ToString()
    {
        return $"{Name}: {string.Join(" + ", Combination)}";
    }
}
