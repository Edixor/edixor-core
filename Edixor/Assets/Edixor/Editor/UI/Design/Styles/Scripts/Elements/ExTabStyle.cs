using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExTabStyle : ExElementStyle<ExTabStyleState>, IStyleEntry
{
    [SerializeField] private string name;
    public string Name => name;

    public ExTabStyle(string name)
    {
        this.name = name;
    }
}