using System;
using System.Collections.Generic;

public class EdixorFunctionRegistry
{
    private Dictionary<Type, EdixorFunction> functions = new Dictionary<Type, EdixorFunction>();

    // Регистрация функции
    public void RegisterFunction(Type functionType, EdixorFunction function)
    {
        if (functions.ContainsKey(functionType))
        {
            throw new InvalidOperationException($"Function {functionType.Name} is already registered.");
        }

        functions.Add(functionType, function);
    }

    // Получение функции по типу
    public EdixorFunction GetFunction<T>() where T : EdixorFunction
    {
        if (functions.TryGetValue(typeof(T), out var function))
        {
            return function;
        }
        throw new InvalidOperationException($"Function {typeof(T).Name} not found.");
    }

    // Активация функции
    public void ActivateFunction<T>() where T : EdixorFunction
    {
        var function = GetFunction<T>();
        function.Activate();
    }
}
