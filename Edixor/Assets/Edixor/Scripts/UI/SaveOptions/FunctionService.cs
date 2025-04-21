using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq; 

public class FunctionService : EdixorSettingTest<FunctionSaveAsset>
{
    private DIContainer container;
    public FunctionService(DIContainer container) : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/FunctionLogicSettings.asset")) { 
        this.container = container;
    }

    public List<FunctionData> GetDataFunctions() => settings.SaveItems;
    public List<Function> GetFunctions() {
        List<Function> functions = new List<Function>();
        foreach (var item in settings.SaveItems)
        {
            FunctionLogic logic = container.ResolveNamed<FunctionLogic>(item.LogicKey);
            functions.Add(new Function(item, logic));
        }
        return functions;
    }

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
