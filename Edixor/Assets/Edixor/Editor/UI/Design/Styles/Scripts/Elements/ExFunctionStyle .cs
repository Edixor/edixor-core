using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExFunctionStyle : ExElementStyle<ExFunctionStyleState>, IStyleEntry
{
    [SerializeField] private string name;
    public string Name => name;

    public ExFunctionStyle(string name)
    {
        this.name = name;
    }
}