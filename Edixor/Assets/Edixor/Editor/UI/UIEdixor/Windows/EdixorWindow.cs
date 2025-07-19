using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Text;
using System;

using ExTools.Edixor.Interface;
using ExTools.Controllers;
using ExTools.Tabs.Basic;
using ExTools.UI.Content;
using ExTools.Settings;

namespace ExTools
{
    public abstract class EdixorWindow : EditorWindow, IMinimizable, IRestartable, IClosable
    {
        private string _title = "None";
        private EdixorRegistrySetting Registry =>
            container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting);

        protected DIContainer container;
        private bool _skipGeometry;
        private Rect? _originalWindowRectBeforeMinimize;
        protected readonly Vector2 minimalSizeThreshold = new Vector2(150, 60);

        private WindowStateSetting _windowStateService;
        protected WindowStateSetting WindowStateSetting => _windowStateService ??= CreateWindowState();

        private IControllersFacade _controllers;
        protected IControllersFacade Controllers => _controllers ??= InitializeControllers();

        private ISettingsFacade _settings;
        protected ISettingsFacade Settings => _settings ??= InitializeSetting();

        private string _windowTypeId;
        public string WindowTypeId
        {
            get
            {
                if (string.IsNullOrEmpty(_windowTypeId))
                    _windowTypeId = ComputeDeterministicId();
                return _windowTypeId;
            }
        }

        private string _windowName;
        public string WindowName
        {
            get
            {
                if (string.IsNullOrEmpty(_windowName))
                    _windowName = ComputeName();
                return _windowName;
            }
        }

        public static void ShowWindow<T>(string title) where T : EditorWindow
        {
            var window = GetWindow<T>(title);
            window.titleContent = new GUIContent(title);
            window.Show();
        }

        private void OnFocus()
        {
            TrySetFocus();
        }

        protected void OnEnable()
        {
            _title = titleContent.text;
            ExDebug.BeginGroup(_title + " is open");

            container = EdixorObjectLocator.LoadObject<DIContainer>("Servers/DIContainer.asset");
            RegisterServices();

            TrySetFocus();

            Registry.AddItem(new EdixorRegistryEntry
            {
                Id = WindowTypeId,
                Title = titleContent.text,
                IsColdStart = true
            });

            var versionTracker = new WindowConfigurationVersion();
            var builder = new WindowConfigurationBuilder(Controllers, Settings, WindowTypeId, versionTracker);

            string oldHash = WindowStateSetting.GetString("LastConfigHash");

            Configure(builder);
            string newHash = versionTracker.ComputeHash();

            if (newHash != oldHash)
            {
                ExDebug.Log("Configuration changed — resetting...");
                Controllers.ResetConfiguration();
                Settings.ResetConfiguration();
                WindowStateSetting.SetString("LastConfigHash", newHash);
            }
            else
            {
                ExDebug.Log("Configuration unchanged — skipping reset");
            }

            EdixorBootstrapper.Bootstrap(
                controllers: Controllers,
                settings: Settings,
                root: rootVisualElement,
                container: container,
                configure: () => Configure(new WindowConfigurationBuilder(Controllers, Settings, WindowTypeId, versionTracker))
            );

            InitializeWindow();
        }

        private string ComputeDeterministicId()
        {
            string key = GetType().AssemblyQualifiedName;
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            return new Guid(hash).ToString();
        }

        private string ComputeName() => GetType().Name;

        protected virtual void Configure(WindowConfigurationBuilder builder)
        {
            builder
                .HotKey("Exit")
                .HotKey("Minimize")
                .HotKey("Restart")

                .Function("HotKey")
                .Function("Setting")
                .Function("Restart")
                .Function("Minimization")

                .Status("Version")
                .Status("Key Combination")

                .BasicTab("NewTab")

                .Layout("Standard")
                .Style("Unity");
        }

        private void RegisterServices()
        {
            container.RegisterNamed<IClosable>(ServiceNameKeys.Edixor_Close + WindowTypeId, this);
            container.RegisterNamed<IMinimizable>(ServiceNameKeys.Edixor_Mini + WindowTypeId, this);
            container.RegisterNamed<IRestartable>(ServiceNameKeys.Edixor_Restart + WindowTypeId, this);

            container.RegisterNamed<InputReader>(ServiceNameKeys.Edixor_Input + WindowTypeId, new InputReader(rootVisualElement));
        }

        private void InitializeWindow()
        {
            var pos = position;
            if (pos.width <= minimalSizeThreshold.x && pos.height <= minimalSizeThreshold.y)
                WindowStateSetting.SetMinimized(true);
            else
                WindowStateSetting.SetWindowOpen(true);
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            var root = rootVisualElement;
            root.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (_skipGeometry) return;
                var p = position;
                if (!WindowStateSetting.GetMinimized() && (p.width <= minimalSizeThreshold.x || p.height <= minimalSizeThreshold.y))
                    MinimizeWindow();
                else if (WindowStateSetting.GetMinimized() && (p.width > minimalSizeThreshold.x && p.height > minimalSizeThreshold.y))
                    ReturnWindowToOriginalSize();
            });
            root.RegisterCallback<KeyDownEvent>(evt => Controllers.OnKeys());
            root.Focus();
        }

        private void TrySetFocus()
        {
            if (Registry.IsRegistered(WindowTypeId))
            {
                Registry.SetCorrectItem(WindowTypeId);
            }
        }

        private void OnGUI()
        {
            if (WindowStateSetting.GetMinimized()) return;
            Controllers.Update();
        }

        public void RestartWindow()
        {
            if (!WindowStateSetting.IsWindowOpen())
            {
                ExDebug.LogWarning("Window is not open, skipping restart.");
                return;
            }

            ExDebug.Log("Restarting window...");
            string windowTitle = titleContent.text;
            var type = GetType();

            EditorApplication.delayCall += () =>
            {
                var method = typeof(EdixorWindow).GetMethod(nameof(ShowWindow)).MakeGenericMethod(type);
                method.Invoke(null, new object[] { windowTitle });
            };

            Close();
        }


        public void MinimizeWindow()
        {
            var r = position;
            if (r.width > minimalSizeThreshold.x && r.height > minimalSizeThreshold.y)
                WindowStateSetting.SetOriginalWindowRect(r);
            WindowStateSetting.SetMinimized(true);
            minSize = minimalSizeThreshold;
            _skipGeometry = true;
            position = new Rect(r.x, r.y, minimalSizeThreshold.x, minimalSizeThreshold.y);
            _skipGeometry = false;
            Controllers.ShowUI(new UIMin());
            ExDebug.Log($"MinimizeWindow saved rect: {r}");
        }

        public void ReturnWindowToOriginalSize()
        {
            var original = WindowStateSetting.GetOriginalWindowRect();
            WindowStateSetting.SetMinimized(false);
            _skipGeometry = true;
            position = original;
            _skipGeometry = false;
            rootVisualElement.Clear();
            Controllers.Initialize();
            ExDebug.Log($"ReturnWindowToOriginalSize restoring rect: {original}");
        }

        public void CloseEdixor() => Close();

        protected virtual void OnDisable()
        {
            Controllers.OnWindowClose();
            WindowStateSetting.SetWindowOpen(false);
            ExDebug.Log(_title + " is disabled");
            ExDebug.EndGroup();
        }

        private WindowStateSetting CreateWindowState()
        {
            var state = new WindowStateSetting(WindowName);
            state.SetRootElement(rootVisualElement);
            return state;
        }

        private IControllersFacade InitializeControllers()
        {
            IControllersFacade facade;
            string controllerKey = ServiceNameKeys.EdixorControllers + WindowTypeId;
            if (!container.IsRegisteredNamed<IControllersFacade>(controllerKey))
            {
                IUIController ui = new UIController(container);
                ITabController tabs = new TabController(container);
                IFunctionController func = new FunctionController(ui, container);
                IStatusController status = new StatusController(ui, container);
                IHotKeyController hkey = new HotKeyController(container, Settings);
                IFactoryHotKey factoryHotKey = new FactoryHotKey();
                IFactoryFunction factoryFunction = new FactoryFunction();
                IFactoryStatus factoryState = new FactoryStatus();

                facade = new ControllersFacade(
                    tabs,
                    ui,
                    func,
                    hkey,
                    status,
                    factoryHotKey,
                    factoryFunction,
                    factoryState
                );

                container.RegisterNamed<IUIController>(ServiceNameKeys.UIController + WindowTypeId, ui);
                container.RegisterNamed<ITabController>(ServiceNameKeys.TabController + WindowTypeId, tabs);
                container.RegisterNamed<IFunctionController>(ServiceNameKeys.FunctionController + WindowTypeId, func);
                container.RegisterNamed<IStatusController>(ServiceNameKeys.StatusController + WindowTypeId, status);
                container.RegisterNamed<IHotKeyController>(ServiceNameKeys.HotKeyController + WindowTypeId, hkey);
                container.RegisterNamed<IControllersFacade>(controllerKey, facade);
            }
            else
            {
                facade = container.ResolveNamed<IControllersFacade>(controllerKey);
            }

            facade.InitOptions(rootVisualElement, container, Settings);
            return facade;
        }

        private ISettingsFacade InitializeSetting()
        {
            var settings = new SettingsFacade(WindowName, WindowTypeId, container);

            string settingsKey = ServiceNameKeys.EdixorSettings + WindowTypeId;
            if (!container.IsRegisteredNamed<ISettingsFacade>(settingsKey))
            {
                container.RegisterNamed<ISettingsFacade>(settingsKey, settings);
            }

            return settings;
        }
    }
}

