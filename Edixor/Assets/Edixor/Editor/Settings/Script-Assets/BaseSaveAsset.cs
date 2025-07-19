using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSaveAsset<T> : ScriptableObject
{
    [SerializeField, SerializeReference]
    private List<T> items = new();

    public List<T> Items
    {
        get => items;
        set => items = value ?? new List<T>();
    }
}