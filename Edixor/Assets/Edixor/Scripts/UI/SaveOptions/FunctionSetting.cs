using HotKeySettingsBase = EdixorSettingsData<FunctionSaveAsset, FunctionData, string>;
using SettingItemFull = ISettingItemFull<Function, string>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

public class FunctionSetting : HotKeySettingsBase, SettingItemFull
{
    private DIContainer container;
    public FunctionSetting(DIContainer container) 
        : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/FunctionLogicSettings.asset")) 
    { 
        this.container = container;
    }

    public override FunctionData[] GetAllItem() => Settings.SaveItems.ToArray();

    public Function[] GetAllItemFull() {
        var functions = new List<Function>();
        foreach (var item in Settings.SaveItems)
        {
            FunctionLogic logic = container.ResolveNamed<FunctionLogic>(item.LogicKey);
            functions.Add(new Function(item, logic));
        }
        return functions.ToArray();
    }

    public override FunctionData GetItem(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new System.ArgumentException("Имя функции не может быть пустым или null.", nameof(key));
        
        if (Settings.SaveItems == null || Settings.SaveItems.Count == 0)
            throw new System.InvalidOperationException("Список функций пуст или не инициализирован.");

        var function = Settings.SaveItems
            .Where(item => item != null)
            .FirstOrDefault(item => string.Equals(item.Name, key, System.StringComparison.OrdinalIgnoreCase));

        if (function == null)
            throw new System.InvalidOperationException($"Функция с именем '{key}' не найдена.");

        return function;
    }


    public Function GetItemFull(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new System.ArgumentException("Имя функции не может быть пустым или null.", nameof(key));
        
        var function = Settings.SaveItems
            .FirstOrDefault(item => item.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));

        if (function == null)
            throw new System.InvalidOperationException($"Функция с именем '{key}' не найдена.");

        FunctionLogic logic = container.ResolveNamed<FunctionLogic>(function.LogicKey);
        return new Function(function, logic);
    }

    public override void AddItem(FunctionData item, string key)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrEmpty(item.Name))
            throw new ArgumentException("Имя функции не может быть пустым или null.", nameof(item));

        // 1. Берём текущий список (копию)
        var list = Settings.SaveItems ?? new List<FunctionData>();

        // 2. Проверяем, нет ли уже:
        //    — того же экземпляра (ReferenceEquals) ИЛИ
        //    — функции с тем же именем (игнорируя регистр)
        if (list.Any(i => ReferenceEquals(i, item) 
                    || i.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Функция с именем '{item.Name}' уже существует.");
        }

        // 3. Вставляем либо в конец, либо сразу после элемента с именем key
        if (string.IsNullOrEmpty(key))
        {
            Debug.Log($"Добавление функции без ключа: {item.Name}");
            list.Add(item);
        }
        else
        {
            int idx = list.FindIndex(i => 
                i != null && i.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (idx < 0)
                throw new InvalidOperationException($"Не найден элемент с именем '{key}', после которого нужно вставить.");

            Debug.Log($"Добавление функции '{item.Name}' после '{key}'");
            list.Insert(idx + 1, item);
        }

        // 4. Записываем изменённый список обратно в Settings и сохраняем ассет
        Settings.SaveItems = list;
        SaveSettings();
    }



    public override void RemoveItem(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new System.ArgumentException("Имя функции не может быть пустым или null.", nameof(key));
        
        var function = Settings.SaveItems
            .FirstOrDefault(item => item.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));

        if (function == null)
            throw new System.InvalidOperationException($"Функция с именем '{key}' не найдена.");

        Settings.SaveItems.Remove(function);
        SaveSettings();
    }

    public override void UpdateItem(string key, FunctionData item)
    {
        if (string.IsNullOrEmpty(key))
            throw new System.ArgumentException("Имя функции не может быть пустым или null.", nameof(key));
        
        var index = Settings.SaveItems
            .FindIndex(i => i.Name.Equals(key, System.StringComparison.OrdinalIgnoreCase));

        if (index == -1)
            throw new System.InvalidOperationException($"Функция с именем '{key}' не найдена.");

        Settings.SaveItems[index] = item;
        SaveSettings();
    }
}
