using UnityEngine.UIElements;
using UnityEngine;
using ExTools;
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
        if (root == null)
        {
            ExDebug.LogError("Root element is null. Cannot initialize EdixorInterfaceFacade.");
            return;
        }
        if (container == null)
        {
            ExDebug.LogError("Container is null. Cannot initialize EdixorInterfaceFacade.");
            return;
        }
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

    public void LoadHotKey(string name, string title = null, Action action = null)
    {
        var hotKeySetting = _container.ResolveNamed<HotKeySetting>(ServiceNames.HotKeySetting);
        KeyAction key = hotKeySetting?.GetItemFull(new HotKeyId(title, name));

        if (key != null)
        {
            _hotKey.AddKey(key);
        }
        else
        {
            ExDebug.LogWarning($"[EdixorInterfaceFacade] Попытка загрузить горячую клавишу \"{name}\" с заголовком \"{title}\", но она не найдена или равна null.");
        }
    }


    public void LoadFunction(string name, string key = null, Action action = null)
    {
        Function func = _container.ResolveNamed<FunctionSetting>(ServiceNames.FunctionSetting).GetItemFull(name);
        _func.AddFunction(func);
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

    public void SetLayout(string path) {

    }
    public void SetStyle(string path) {

    }

    public void BeginWindowSize(Vector2 size) {

    }
    public void FixedWindowSize(Vector2 size) {

    }
    public void BeginWindowPosition(Vector2 position) {

    }
    public void FixedWindowPosition(Vector2 position) {

    }
}