using System.Collections.Generic;
using UnityEngine.UIElements;
using ExTools;
using System.Linq;

public class FunctionController : IFunctionController
{
    private IUIController _ui;
    private DIContainer _container;
    private LayoutSetting _layoutSetting;
    private List<Function> _functions = new List<Function>();
    private FactoryUIFunction _factory = new FactoryUIFunction();

    public FunctionController(IUIController ui, DIContainer container)
    {
        _ui = ui;
        _container = container;
        _layoutSetting = container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting);
    }

    public void Initialize(IUIController uiBase = null)
    {
        _ui = uiBase ?? _container.ResolveNamed<IUIController>(ServiceNames.UIController);
    }

    public void RestoreFunction()
    {
        ExDebug.BeginGroup("FunctionController: restore function");

        ExDebug.Log("List functions:" );
        foreach (var func in _functions)
        {
            ExDebug.Log($"Initializing function '{func.Data.Name}'.");
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

            var containerElement = _ui.GetElement(element.elementName);
            if (containerElement == null)
            {
                ExDebug.LogWarning($"FunctionController: Container '{element.elementName}' not found in UI.");
                continue;
            }

            _factory.Init(containerElement);

            foreach (var name in element.functionNames)
            {
                var func = _functions.FirstOrDefault(f => f.Data.Name == name);
                if (func == null) continue;
                _factory.Create(func);
            }

            if (!containerElement.Children().Any())
            {
                ExDebug.LogError($"FunctionController: Container '{element.elementName}' is empty after adding buttons.");
            }
        }

        _container.Resolve<StyleLogicEdixor>().FunctionStyling(_factory.GetItems());
        ExDebug.EndGroup();
    }

    public void AddFunction(Function function)
    {
        if (function == null)
        {
            ExDebug.LogWarning("Attempt to add null Function.");
            return;
        }

        var name = function.Data?.Name;
        if (string.IsNullOrEmpty(name))
        {
            ExDebug.LogWarning("Function.Data.Name is null or empty.");
            return;
        }

        if (_functions.Any(f => f.Data?.Name == name))
        {
            ExDebug.LogWarning($"Function with Name '{name}' is already added.");
            return;
        }

        function.Logic.SetContainer(_container);
        function.Logic.Init();
        _functions.Add(function);
    }

    public void RemoveFunction(string functionName)
    {
        var function = _functions.FirstOrDefault(f => f.Data?.Name == functionName);
        if (function == null)
        {
            ExDebug.LogWarning($"No Function found with Name '{functionName}'.");
            return;
        }
        _functions.Remove(function);
    }

    public void Execute(string functionName)
    {
        ExDebug.LogWarning($"Execute('{functionName}') is not implemented.");
    }
}