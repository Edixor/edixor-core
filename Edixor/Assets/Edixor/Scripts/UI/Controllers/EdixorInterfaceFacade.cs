using UnityEngine.UIElements;
using System;
public class EdixorInterfaceFacade : IEdixorInterfaceFacade
{
    private readonly ITabController _tabs;
    private readonly IUIController _ui;
    private readonly IFunctionController _func;
    private readonly IHotKeyController _hotKey;
    private readonly IFactoryHotKey _factoryHotKey;
    private readonly IFactoryFunction _factoryFunction;
    private VisualElement _root;
    private DIContainer _container;

    public EdixorInterfaceFacade(ITabController tabs, IUIController ui, IFunctionController func, IHotKeyController hotKey, IFactoryHotKey factoryHotKey, IFactoryFunction factoryFunction)
    {
        _ui = ui;
        _tabs = tabs;
        _func = func;
        _hotKey = hotKey;
        _factoryHotKey = factoryHotKey;
        _factoryFunction = factoryFunction;
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
        _func.InitFunction(_root);
    }

    public void AddHotKey(KeyAction key)
    {
        _hotKey.AddKey(key);
    }

    public void LoadHotKey(string path, string title, Action action = null)
    {
       _factoryHotKey.CreateFromAssets(path, new HotKeyId(title), action);
    }

    public void LoadFunction(string path, string key = null, Action action = null)
    {
        _factoryFunction.CreateFromAssets(path, key, action);
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