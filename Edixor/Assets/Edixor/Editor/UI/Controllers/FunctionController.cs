using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using ExTools;

public class FunctionController : IFunctionController
{
    private readonly DIContainer _container;
    private readonly FunctionSetting _functionSetting;
    private readonly LayoutSetting _layoutSetting;
    private readonly List<Function> _functions = new List<Function>();
    private readonly FactoryUIFunction _factory = new FactoryUIFunction();

    public FunctionController(DIContainer container)
    {
        _container = container;
        _layoutSetting = _container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting);
    }

    public void InitFunction(VisualElement root)
    {
        if (root == null)
        {
            ExDebug.LogWarning("FunctionController: root VisualElement is null. Aborting InitFunction.");
            return;
        }

        _factory.Init(root);

        foreach (Function item in _functions)
        {
            if(!item.Logic.Empty()) 
            {
                item.Logic.SetContainer(_container);
            }
        }

        var parameters = _layoutSetting.GetCorrectItem()?.AssetParameters;
        if (parameters == null)
        {
            ExDebug.LogWarning("FunctionController: LayoutParameters is null. Nothing to create.");
            return;
        }

        foreach (var element in parameters.Elements)
        {

            if (element?.functionNames == null) continue;

            foreach (string name in element.functionNames)
            {
                if (string.IsNullOrEmpty(name)) continue;

                var function = _functions.FirstOrDefault(f => f.Data?.Name == name);
                if (function == null) continue;

                _factory.Create(function, element.elementName);
            }
        }
    }

    public void AddFunction(Function function)
    {
        if (function == null)
        {
            ExDebug.LogWarning("FunctionController: Attempt to add null Function.");
            return;
        }

        var name = function.Data?.Name;
        if (string.IsNullOrEmpty(name))
        {
            ExDebug.LogWarning("FunctionController: Function.Data.Name is null or empty.");
            return;
        }

        if (_functions.Any(f => f.Data?.Name == name))
        {
            ExDebug.LogWarning($"FunctionController: Function with Name '{name}' is already added.");
            return;
        }

        function.Logic.SetContainer(_container);
        function.Logic.Init();
        _functions.Add(function);
    }

    public void RemoveFunction(string functionName)
    {
        if (string.IsNullOrEmpty(functionName))
        {
            ExDebug.LogWarning("FunctionController: functionName is null or empty.");
            return;
        }

        var function = _functions.FirstOrDefault(f => f.Data?.Name == functionName);
        if (function == null)
        {
            ExDebug.LogWarning($"FunctionController: No Function found with Name '{functionName}'.");
            return;
        }

        _functions.Remove(function);
    }

    public void Execute(string functionName)
    {
        ExDebug.LogWarning($"FunctionController: Execute('{functionName}') is not implemented.");
    }
}
