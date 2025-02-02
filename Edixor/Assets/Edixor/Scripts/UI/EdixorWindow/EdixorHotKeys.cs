using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class EdixorHotKeys
{
    private readonly EdixorWindow window;
    private List<KeyAction> hotkeyActions; // Список комбинаций и их действий
    private HashSet<KeyCode> currentlyPressedKeys; // Состояние всех нажатых клавиш
    private HashSet<List<KeyCode>> activatedCombinations; // Защита от двойной активации

    public EdixorHotKeys(EdixorWindow window)
    {
        this.window = window;

        hotkeyActions = window.GetSetting().GetHotKeys();

        // Сортируем комбинации по убыванию длины
        hotkeyActions = hotkeyActions.OrderByDescending(a => a.Combination.Count).ToList();

        currentlyPressedKeys = new HashSet<KeyCode>();
        activatedCombinations = new HashSet<List<KeyCode>>(new KeyCombinationComparer());
    }

    public void Init() {
        
    }

    public void OnKeys()
    {
        Event e = Event.current;

        // Обновляем состояние клавиш
        if (e.type == EventType.KeyDown && !currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Add(e.keyCode);

            // Проверяем комбинации клавиш
            foreach (var action in hotkeyActions)
            {
                if (IsCombinationPressed(action.Combination) && !activatedCombinations.Contains(action.Combination))
                {
                    action.Action(); // Вызываем действие
                    activatedCombinations.Add(action.Combination); // Помечаем комбинацию как активированную
                    e.Use(); // Указываем, что событие обработано
                    return; // Прекращаем проверку, чтобы избежать двойной активации
                }
            }
        }
        else if (e.type == EventType.KeyUp && currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Remove(e.keyCode);

            // Сбрасываем активированные комбинации, если одна из её клавиш отпущена
            activatedCombinations.RemoveWhere(combination => combination.Contains(e.keyCode));
        }
    }

    private bool IsCombinationPressed(List<KeyCode> combination)
    {
        // Проверяем, содержатся ли все клавиши комбинации в текущем наборе
        foreach (var key in combination)
        {
            if (!currentlyPressedKeys.Contains(key))
            {
                return false;
            }
        }
        return true;
    }

    // Класс для сравнения комбинаций клавиш
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
