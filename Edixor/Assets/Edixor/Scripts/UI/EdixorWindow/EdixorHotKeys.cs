using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdixorHotKeys
{
    private readonly EdixorWindow window;
    private List<KeyAction> hotkeyActions;
    private HashSet<KeyCode> currentlyPressedKeys;
    private HashSet<List<KeyCode>> activatedCombinations;

    public EdixorHotKeys(EdixorWindow window)
    {
        this.window = window;
        hotkeyActions = window.GetSetting().GetHotKeys();

        // Передаём окно в каждое действие
        foreach (var action in hotkeyActions)
        {
            action.SetWindow(window);
        }

        // Сортировка по длине комбинации (опционально)
        hotkeyActions = hotkeyActions.OrderByDescending(a => a.Combination.Count).ToList();

        currentlyPressedKeys = new HashSet<KeyCode>();
        activatedCombinations = new HashSet<List<KeyCode>>(new KeyCombinationComparer());
    }

    public void OnKeys()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown && !currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Add(e.keyCode);
            foreach (var action in hotkeyActions)
            {
                if (IsCombinationPressed(action.Combination) && !activatedCombinations.Contains(action.Combination))
                {
                    if (action.enable)
                    {
                        action.Execute();
                        activatedCombinations.Add(action.Combination);
                        e.Use();
                        return;
                    }
                }
            }
        }
        else if (e.type == EventType.KeyUp && currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Remove(e.keyCode);
            activatedCombinations.RemoveWhere(combination => combination.Contains(e.keyCode));
        }
    }

    private bool IsCombinationPressed(List<KeyCode> combination)
    {
        return combination.All(key => currentlyPressedKeys.Contains(key));
    }

    public List<KeyAction> GetKeys() {
        return hotkeyActions;
    }

    private class KeyCombinationComparer : IEqualityComparer<List<KeyCode>>
    {
        public bool Equals(List<KeyCode> x, List<KeyCode> y)
        {
            if (x == null || y == null || x.Count != y.Count)
                return false;
            for (int i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }

        public int GetHashCode(List<KeyCode> obj)
        {
            if (obj == null)
                return 0;
            int hash = 17;
            foreach (var key in obj)
            {
                hash = hash * 31 + key.GetHashCode();
            }
            return hash;
        }
    }
}
