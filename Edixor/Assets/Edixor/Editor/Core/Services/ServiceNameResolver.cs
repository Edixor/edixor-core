using ExTools.Settings;

public class ServiceNameResolver
{
    private readonly DIContainer _container;
    private string _id;

    public ServiceNameResolver(DIContainer container)
    {
        _container = container;
    }

    private string Id
    {
        get
        {
            if (_id == null)
            {
                var setting = _container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting);
                _id = setting.GetCorrectItem().Id;
            }
            return _id;
        }
    }

    public string EdixorControllers => "EdixorControllers: " + Id;
    public string EdixorSettings => "EdixorSettings: " + Id;
    public string UIController => "UIController: " + Id;
    public string TabController => "TabController: " + Id;
    public string FunctionController => "FunctionController: " + Id;
    public string StatusController => "StatusController: " + Id;
    public string HotKeyController => "HotKeyController: " + Id;
    public string Edixor_Close => "Edixor_Closable: " + Id;
    public string Edixor_Mini => "Edixor_Minimizable: " + Id;
    public string Edixor_Restart => "Edixor_Restart: " + Id;
    public string Edixor_Input => "Edixor_Input: " + Id;
}
