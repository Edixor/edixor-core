using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class Factory<TBase, TContext>
{
    private List<TBase> _registeredItems = new List<TBase>();

    public void RegisterAll(TContext context)
    {
        _registeredItems.Clear();

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
        }
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
}
