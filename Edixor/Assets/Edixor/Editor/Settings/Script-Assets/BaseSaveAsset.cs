using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSaveAsset<T> : ScriptableObject
{
    [SerializeField, SerializeReference]
    private List<T> items = new();

    public virtual List<T> Items
    {
        get => new List<T>(items);
        set => items = value != null ? new List<T>(value) : new List<T>();
    }
}
