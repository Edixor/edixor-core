using System;
using System.Collections.Generic;
using UnityEngine;
using ExTools;

public abstract class ListControllerBase<TItem>
{
    protected readonly List<TItem> items = new();

    public virtual void AddItem(TItem item)
    {
        if (item == null)
        {
            ExDebug.LogWarning("Attempted to add null item.");
            return;
        }

        if (items.Contains(item))
        {
            ExDebug.LogWarning("Item already exists in the list.");
            return;
        }

        items.Add(item);
    }

    public virtual void RemoveItem(TItem item)
    {
        if (item == null) return;
        items.Remove(item);
    }

    public virtual void ClearAll()
    {
        items.Clear();
    }

    public abstract void Process();
}
