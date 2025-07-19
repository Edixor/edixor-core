using System;
using System.Collections.Generic;
using UnityEngine;
using ExTools;
public abstract class SingleItemControllerBase<TItem>
{
    protected TItem item;

    public virtual void AddItem(TItem newItem)
    {
        if (newItem == null)
        {
            ExDebug.LogWarning("Attempting to add a null element to a singleton controller.");
            return;
        }

        item = newItem;
        OnItemSet(newItem);
    }

    protected abstract void OnItemSet(TItem newItem);
}