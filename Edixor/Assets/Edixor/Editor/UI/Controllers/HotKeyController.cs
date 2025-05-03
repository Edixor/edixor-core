using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

public class HotKeyController : IHotKeyController
{
    private readonly DIContainer _container;
    private readonly List<KeyAction> hotkeyActions = new List<KeyAction>();
    private HashSet<KeyCode> currentlyPressedKeys;
    private HashSet<List<KeyCode>> activatedCombinations;
    private bool isInitialized = false;

    public HotKeyController(DIContainer container) {
        _container = container;
    }

    public void InitHotKeys()
    {
        currentlyPressedKeys = new HashSet<KeyCode>();
        activatedCombinations = new HashSet<List<KeyCode>>(new KeyCombinationComparer());

        foreach (KeyAction item in hotkeyActions)
        {
            if(!item.Logic.Empty()) 
            {
                item.Logic.SetContainer(_container);
            }
        }

        isInitialized = true;
    }

    public bool IsHotKeyEnabled()
    {
        return isInitialized;
    }

    public void OnKeys()
    {
        var e = Event.current;
        if (e == null) return;

        if (e.type == EventType.KeyDown && !currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Add(e.keyCode);

            foreach (var action in hotkeyActions)
            {
                var combo = action.Data?.Combination;
                if (combo == null) continue;

                if (combo.All(k => currentlyPressedKeys.Contains(k))
                    && !activatedCombinations.Contains(combo))
                {
                    if (action.Data.enable)
                    {
                        action.Execute();
                        activatedCombinations.Add(combo);
                        e.Use();
                        return;
                    }
                }
            }
        }
        else if (e.type == EventType.KeyUp && currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Remove(e.keyCode);
            activatedCombinations.RemoveWhere(c => c.Contains(e.keyCode));
        }
    }

    public void AddKey(KeyAction key)
    {
        if (key == null || key.Data?.Combination == null)
            return;
        key.Logic.SetContainer(_container);
        hotkeyActions.Add(key);
    }

    private class KeyCombinationComparer : IEqualityComparer<List<KeyCode>>
    {
        public bool Equals(List<KeyCode> x, List<KeyCode> y)
        {
            if (x == null || y == null || x.Count != y.Count) return false;
            for (int i = 0; i < x.Count; i++)
                if (x[i] != y[i]) return false;
            return true;
        }
        public int GetHashCode(List<KeyCode> obj)
        {
            if (obj == null) return 0;
            int hash = 17;
            foreach (var key in obj)
                hash = hash * 31 + key.GetHashCode();
            return hash;
        }
    }
}
