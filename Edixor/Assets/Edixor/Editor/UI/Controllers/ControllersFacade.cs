using UnityEngine.UIElements;
using UnityEngine;
using System;

using ExTools.UI.Content;
using ExTools.Settings;
using ExTools;

namespace ExTools.Controllers
{
    public class ControllersFacade : IControllersFacade
    {
        private ISettingsFacade _settings;
        private readonly ITabController _tabs;
        private readonly IUIController _ui;
        private readonly IFunctionController _func;
        private readonly IHotKeyController _hotKey;
        private IStatusController _status;
        private IFactoryHotKey _factoryHotKey;
        private IFactoryFunction _factoryFunction;
        private IFactoryStatus _factoryState;
        private VisualElement _root;
        private DIContainer _container;
        public bool IsInitialized { get; private set; } = false;

        public ControllersFacade(ITabController tabs, IUIController ui, IFunctionController func,
            IHotKeyController hotKey, IStatusController status, IFactoryHotKey factoryHotKey, IFactoryFunction factoryFunction, IFactoryStatus factoryState)
        {
            _tabs = tabs; _ui = ui; _func = func; _hotKey = hotKey; _status = status;
            _factoryHotKey = factoryHotKey; _factoryFunction = factoryFunction; _factoryState = factoryState;
        }

        public void InitOptions(VisualElement root, DIContainer container, ISettingsFacade settings)
        {
            _settings = settings;
            _container = container;
            _root = root;
        }

        public void Initialize()
        {
            _ui.InitRoot(_root);
            _ui.Show(new UIRoot());
            _func.Initialize(_ui);
            _tabs.Initialize(_ui, _settings.TabSetting);
            _status.Initialize(_ui, _container);
            _func.Process();
            _tabs.RestoreTabs();
            _status.Process();
            _hotKey.InitHotKeys();

            IsInitialized = true;
        }

        public void ResetConfiguration()
        {
            _func.ResetConfiguration();
            _hotKey.ResetConfiguration();
            _status.ResetConfiguration();

            IsInitialized = false;
        }

        public void ShowUI(UIContent content)
        {
            if (content == null)
            {
                ExDebug.LogError("Content is null. Cannot show UI.");
                return;
            }

            _ui.Show(content);
        }

        public void LoadHotKey(string name, string title = null, Action action = null)
        {
            if (IsInitialized) return;
            _factoryHotKey.InitializeData(_settings.HotKeySetting, (HotKeyController)_hotKey);
            _factoryHotKey.CreateFromAssets($"HotKeys/{name}.asset", new HotKeyId(title, name), action);
        }

        public void LoadFunction(string name, string key = null, Action action = null)
        {
            if (IsInitialized) return;
            _factoryFunction.InitializeData(_settings.FunctionSetting, (FunctionController)_func);
            _factoryFunction.CreateFromAssets($"Functions/{name}.asset", key, action);
        }

        public void LoadStatus(string name, string key = null, Action action = null)
        {
            if (IsInitialized) return;
            _factoryState.InitializeData(_settings.StatusSetting, (StatusController)_status);
            _factoryState.CreateFromAssets($"Status/{name}.asset", key, action);
        }
        public EdixorTab GetLogicFor(TabData data) =>
            ((TabController)_tabs).GetLogicFor(data);

        public void AddFunction(Function function) => _func.AddItem(function);
        public void AddStatus(Status status) => _status.AddItem(status);
        public void AddTab(TabData tab, bool saveState, bool autoSwitch = true) => _tabs.AddTab(tab, saveState, autoSwitch);
        public void AddTab(EdixorTab tab, bool saveState, bool autoSwitch = true) => _tabs.AddTab(tab, saveState, autoSwitch);
        public void BasicTab(TabData tab) => _tabs.SetBasicTab(tab);
        public void OnKeys() => _hotKey.OnKeys();
        public void Update()
        {
            _tabs.OnGUI();
            _status.OnGUI();
        }
        public void OnWindowClose() => _tabs.OnWindowClose();
    }

}
