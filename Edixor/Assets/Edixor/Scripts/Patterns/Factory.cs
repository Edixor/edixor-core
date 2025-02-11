#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Factory<TBase, TContext>
{
    private List<TBase> _registeredItems = new List<TBase>();
    private List<string> _registeredPaths = new List<string>();

    public void RegisterAll(TContext context)
    {
        _registeredItems.Clear();
        _registeredPaths.Clear();

        var sortedTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TBase)) && !t.IsAbstract)
            .Select(type => new
            {
                Type = type,
                Order = type.GetCustomAttribute<OrderAttributeFactory>()?.Order ?? int.MaxValue // Если нет атрибута — ставим в конец
            })
            .OrderBy(x => x.Order) // Сортируем по порядку
            .Select(x => x.Type)
            .ToList();

        foreach (var itemType in sortedTypes)
        {
            var constructor = itemType.GetConstructor(new Type[] { typeof(TContext) });
            if (constructor == null)
                throw new InvalidOperationException($"Класс {itemType.Name} должен иметь конструктор с параметром {typeof(TContext).Name}.");

            var instance = (TBase)constructor.Invoke(new object[] { context });
            _registeredItems.Add(instance);

            string assetPath = "Не найден";
#if UNITY_EDITOR
            // Ищем MonoScript, который соответствует данному типу
            var script = MonoImporter.GetAllRuntimeMonoScripts()
                .FirstOrDefault(ms => ms.GetClass() == itemType);
            if (script != null)
            {
                assetPath = AssetDatabase.GetAssetPath(script);
            }
#endif
            _registeredPaths.Add(assetPath);
        }

        // Выводим пути в консоль
#if UNITY_EDITOR
        Debug.Log("Зарегистрированные классы:");
        foreach (var path in _registeredPaths)
        {
            Debug.Log(path);
        }
#else
        Console.WriteLine("Зарегистрированные классы:");
        foreach (var path in _registeredPaths)
        {
            Console.WriteLine(path);
        }
#endif
    }

    public List<TBase> GetAllItems() => _registeredItems;

    // Метод для получения объекта по его типу
    public T GetItem<T>() where T : TBase
    {
        var item = _registeredItems.OfType<T>().FirstOrDefault();
        if (item == null)
            throw new InvalidOperationException($"Объект типа {typeof(T).Name} не найден.");
        return item;
    }

    public string[] GetPaths() => _registeredPaths.ToArray();
}
