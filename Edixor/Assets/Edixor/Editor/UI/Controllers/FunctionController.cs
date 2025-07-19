using System.Linq;
using UnityEngine.UIElements;
using ExTools;

using ExTools.Controllers;
using ExTools.Settings;
public class FunctionController : ListControllerBase<Function>, IFunctionController
{
    private IUIController _ui;
    private DIContainer _container;
    private LayoutSetting _layoutSetting;
    private StyleSetting _styleSetting;
    private FunctionUIFactory _factory = new FunctionUIFactory();

    public FunctionController(IUIController ui, DIContainer container)
    {
        _ui = ui;
        _container = container;
        _layoutSetting = container.ResolveNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting);
        _styleSetting = container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting);
    }

    public void Initialize(IUIController ui, DIContainer container = null)
    {
        if (ui == null)
        {
            ExDebug.LogError("UIController is null. Cannot initialize FunctionController.");
            return;
        }

        if (container != null)
        {
            _container = container;
        }

        _ui = ui;
    }
    public override void AddItem(Function function)
    {
        if (function == null || string.IsNullOrEmpty(function.Data?.Name))
        {
            ExDebug.LogWarning("Attempt to add invalid Function.");
            return;
        }

        if (items.Any(f => f.Data.Name == function.Data.Name))
        {
            bool coldStart = _container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting).GetCorrectItem().IsColdStart;
            if(!coldStart) ExDebug.LogWarning($"Function '{function.Data.Name}' already added.");
            else ExDebug.Log($"Function '{function.Data.Name}' already added.");
            return;
        }

        function.Logic.SetContainer(_container);
        function.Logic.Init();
        base.AddItem(function);
    }

     
     
     
    public override void Process()
    {
        ExDebug.BeginGroup("FunctionController: restore function");

        ExDebug.Log("List functions:");
        foreach (var func in items)
            ExDebug.Log($"Initializing function '{func.Data.Name}'");

        var parameters = _layoutSetting.GetCorrectItem()?.AssetParameters;
        if (parameters == null)
        {
            ExDebug.LogWarning("LayoutParameters is null. Nothing to create.");
            return;
        }

        foreach (var element in parameters.Elements)
        {
            if (element?.functionNames == null) continue;

            var containerElement = _ui.GetElement(element.elementName);
            if (containerElement == null)
            {
                ExDebug.LogWarning($"Container '{element.elementName}' not found.");
                continue;
            }

            _factory.Init(containerElement, _styleSetting.GetCorrectItem().GetEdixorParameters().GetStyleByName(_styleSetting.GetCorrectItem().GetEdixorParameters().Functions, "normal"));
            foreach (var name in element.functionNames)
            {
                var func = items.FirstOrDefault(f => f.Data.Name == name);
                if (func != null)
                    _factory.Create(func);
            }

            if (!containerElement.Children().Any())
                ExDebug.LogError($"Container '{element.elementName}' is empty after adding buttons.");
        }

        ExDebug.EndGroup();
    }

     
     
     
    public void ResetConfiguration()
    {
        ExDebug.Log("ResetConfiguration - clearing all functions and UI elements");
        ClearAll();
        _factory = new FunctionUIFactory();
    }

     
     
     
    public void Execute(string functionName)
    {
        ExDebug.LogWarning($"Execute('{functionName}') not implemented.");
    }
}