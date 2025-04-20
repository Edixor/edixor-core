using UnityEngine.UIElements;
public class EdixorInterfaceFacade : IEdixorInterfaceFacade
{
    private readonly ITabController _tabs;
    private readonly IUIController _ui;
    private readonly IFunctionController _func;
    private readonly IHotKeyController _hotKey;
    private VisualElement _root;
    private DIContainer _container;

    public EdixorInterfaceFacade(ITabController tabs, IUIController ui, IFunctionController func, IHotKeyController hotKey)
    {
        _tabs = tabs;
        _ui = ui;
        _func = func;
        _hotKey = hotKey;
    }

    public void InitOptions(VisualElement root, DIContainer container)
    {
        _container = container;
        _root = root;
    }

    public void Initialize()
    {
        _ui.InitRoot(_root);
        _ui.Show(new UIRoot(_container));
        _tabs.Initialize(_ui);
        _tabs.RestoreTabs();
        _hotKey.InitHotKeys();
    }

    public void AddHotKey(KeyAction key)
    {
        _hotKey.AddKey(key);
    }

    public void AddTab(EdixorTab tab, bool saveState, bool autoSwitch = true)
    {
        _tabs.AddTab(tab, saveState: true, autoSwitch: autoSwitch);
    }

    public void OnKeys()
    {
        _hotKey.OnKeys();
    }

    public void Update()
    {
        _tabs.OnGUI();
    }

    public void OnWindowClose()
    {
        _tabs.OnWindowClose();
    }
}