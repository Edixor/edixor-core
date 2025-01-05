using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System;
public class EdixorFunctionFactory
{
    private EdixorFunctionRegistry _registry = new EdixorFunctionRegistry();
    private List<EdixorFunction> _registeredFunctions = new List<EdixorFunction>();

    // Метод для регистрации всех функций с передачей окна
    public void RegisterAllFunctions(EdixorWindow window)
    {
        // Находим все типы функций, которые наследуют от EdixorFunction и не являются абстрактными
        var functionTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(EdixorFunction)) && !t.IsAbstract)
            .ToList();

        // Для каждого типа создаем экземпляр, передавая в конструктор window
        foreach (var functionType in functionTypes)
        {
            // Проверка на наличие конструктора с параметром EdixorWindow
            var constructor = functionType.GetConstructor(new Type[] { typeof(EdixorWindow) });

            if (constructor != null)
            {
                // Проверяем, была ли эта функция уже зарегистрирована
                if (_registeredFunctions.Any(f => f.GetType() == functionType))
                {
                    Debug.LogWarning($"Function {functionType.Name} is already registered.");
                    continue; // Пропускаем регистрацию этой функции
                }

                // Создаем экземпляр функции, передав window в конструктор
                var function = (EdixorFunction)constructor.Invoke(new object[] { window });
                _registry.RegisterFunction(functionType, function);
                _registeredFunctions.Add(function); // Добавляем в список
            }
            else
            {
                Debug.LogError($"Function {functionType.Name} does not have a constructor that accepts EdixorWindow");
            }
        }
    }

    // Получить зарегистрированную функцию по типу
    public EdixorFunction GetFunction<T>() where T : EdixorFunction
    {
        return _registry.GetFunction<T>();
    }

    // Активация функции
    public void ActivateFunction<T>() where T : EdixorFunction
    {
        _registry.ActivateFunction<T>();
    }

    // Получить все зарегистрированные функции
    public List<EdixorFunction> GetAllFunctions()
    {
        return new List<EdixorFunction>(_registeredFunctions);
    }
}
