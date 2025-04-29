using UnityEngine.UIElements;

public class FunctionController : IFunctionController
{
    private DIContainer _container;
    private readonly FunctionSetting _functionSetting;
    private readonly LayoutSetting _layoutSetting;
    private FactoryUIFunction factory = new FactoryUIFunction();

    public FunctionController(DIContainer container)
    {
        _container = container;
        _functionSetting = _container.ResolveNamed<FunctionSetting>(ServiceNames.FunctionSetting);
        _layoutSetting = _container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting);
    }

    public void InitFunction(VisualElement root = null)
    {
        factory.Init(root);
        
        LayoutParameters parameters = _layoutSetting.GetCorrectItem().AssetParameters;
        foreach (var element in parameters.Elements)
        {
            foreach (string name in element.functionNames)
            {
                Function function = _functionSetting.GetItemFull(name);
                factory.Create(function, element.elementName);
            }
        }
    }

    public void AddFunction() {

    }
    public void RemoveFunction(string functionId) {

    }
    public void Execute(string functionId) {
        
    }
}
