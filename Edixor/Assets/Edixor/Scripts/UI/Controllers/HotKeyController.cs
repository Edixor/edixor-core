using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HotKeyController : IHotKeyController
{
    private DIContainer container;
    private HotKeyService keyService;
    private IFactory factoryBuilder;
    private List<KeyAction> hotkeyActions = new List<KeyAction>();
    private HashSet<KeyCode> currentlyPressedKeys;
    private HashSet<List<KeyCode>> activatedCombinations;
    private bool isInitialized = false;

    public HotKeyController(DIContainer container)
    {
        this.container = container;
    }

    public void InitHotKeys()
    {
        Debug.Log("HotKeyController: Инициализация...");

        keyService = container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting);
        if (keyService == null)
        {
            Debug.LogError("HotKeyController: HotKeyService не был найден в контейнере!");
            return;
        }

        hotkeyActions = keyService.GetAllHotKeys()
            .Select(hotKeyData => new KeyAction(hotKeyData, container))
            .ToList();

        Debug.Log($"HotKeyController: Получено {hotkeyActions.Count} горячих клавиш.");

        foreach (var action in hotkeyActions)
        {
            if (action == null)
            {
                Debug.LogWarning("HotKeyController: Найден null-элемент в hotkeyActions.");
                continue;
            }
            if (action.keyActionLogic != null)
            {
                action.keyActionLogic.SetContainer(container);
            }
            else
            {
                Debug.LogWarning($"HotKeyController: У действия {action} отсутствует KeyActionLogic.");
            }
        }

        currentlyPressedKeys = new HashSet<KeyCode>();
        activatedCombinations = new HashSet<List<KeyCode>>(new KeyCombinationComparer());

        isInitialized = true;
    }

    public bool IsHotKeyEnabled()
    {
        return isInitialized;
    }

    public void OnKeys()
    {
        Event e = Event.current;
        if (e == null) return;
        

        if (e.type == EventType.KeyDown && !currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Add(e.keyCode);

            foreach (var action in hotkeyActions)
            {
                if (action?.keyActionData?.Combination == null)
                {
                    Debug.LogWarning($"HotKeyController: Пропущено действие {action} с null-комбинацией.");
                    continue;
                }


                if (IsCombinationPressed(action.keyActionData.Combination) &&
                    !activatedCombinations.Contains(action.keyActionData.Combination))
                {
                    Debug.Log($"HotKeyController: Комбинация {string.Join(" + ", action.keyActionData.Combination)} активирована.");

                    if (action.keyActionData.enable)
                    {
                        Debug.Log($"HotKeyController: Выполняем действие {action}.");
                        action.Execute();
                        activatedCombinations.Add(action.keyActionData.Combination);
                        e.Use();
                        return;
                    }
                    else
                    {
                        Debug.LogWarning($"HotKeyController: Действие {action} отключено.");
                    }
                }
            }
        }
        else if (e.type == EventType.KeyUp && currentlyPressedKeys.Contains(e.keyCode))
        {
            currentlyPressedKeys.Remove(e.keyCode);
            Debug.Log($"HotKeyController: Отпущена клавиша {e.keyCode}, очищаем активные комбинации.");
            activatedCombinations.RemoveWhere(combination => combination.Contains(e.keyCode));
        }
    }

    private bool IsCombinationPressed(List<KeyCode> combination)
    {
        bool result = combination != null && combination.All(key => currentlyPressedKeys.Contains(key));
        Debug.Log($"HotKeyController: Проверка комбинации {string.Join(" + ", combination)} - {(result ? "нажата" : "не нажата")}");
        return result;
    }

    public void AddKey(KeyAction key)
    {
        if (key == null || key.keyActionData?.Combination == null)
        {
            Debug.LogWarning("HotKeyController: Попытка добавить null-элемент или элемент без комбинации.");
            return;
        }

        hotkeyActions.Add(key);
    }

    public List<KeyAction> GetKeys()
    {
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
