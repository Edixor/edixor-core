using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; 

public class FunctionService : EdixorSetting<FunctionSaveAsset>
{
    public FunctionService() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/FunctionLogicSettings.asset")) { }

    public List<FunctionData> GetFunctions() => settings.SaveItems;

    public FunctionData GetCorrectFunction(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
        {
            throw new System.ArgumentException("Имя функции не может быть пустым или null.", nameof(functionName));
        }
        var function = settings.SaveItems
            .FirstOrDefault(item => item.Name.Equals(functionName, System.StringComparison.OrdinalIgnoreCase));

        if (function == null)
        {
            throw new System.InvalidOperationException($"Функция с именем '{functionName}' не найдена.");
        }

        return function;
    }
}
