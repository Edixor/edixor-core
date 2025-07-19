using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Text;
using System.Linq;
using System;

using ExTools.Edixor.Interface;
using ExTools.Tabs.Advanced;
using ExTools.Controllers;
using ExTools.Settings;

namespace ExTools
{
    public class EdixorInspector : Editor, IRestartable, IClosable
    {
        private bool _hadFocus = false;
        private VisualElement _root;
        private WindowConfigurationBuilder _builder;
        protected DIContainer Container;

        public static EdixorInspector CurrentInspector { get; private set; }

        private string _inspectionTypeId;
        public string InspectionTypeId
        {
            get
            {
                if (string.IsNullOrEmpty(_inspectionTypeId))
                    _inspectionTypeId = ComputeDeterministicId();
                return _inspectionTypeId;
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

        private ISettingsFacade _settings;
        protected ISettingsFacade Settings => _settings ??= InitializeSetting();

        private IControllersFacade _controllers;
        protected IControllersFacade Controllers => _controllers ??= InitializeControllers();

        private WindowStateSetting _windowStateService;
        protected WindowStateSetting WindowStateSetting => _windowStateService ??= CreateWindowState();

        private EdixorRegistrySetting Registry => Container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting);

        public override VisualElement CreateInspectorGUI()
        {
            CurrentInspector = this;

            _inspectionTypeId = ComputeDeterministicId();
            _windowName = ComputeName();

            Container = EdixorObjectLocator.LoadObject<DIContainer>("Servers/DIContainer.asset");
            RegisterServices();

            Registry.AddItem(new EdixorRegistryEntry
            {
                Id = _inspectionTypeId,
                Title = _windowName,
                IsColdStart = true
            });

            _root = new VisualElement
            {
                name = "EdixorRoot",
                focusable = true,
                pickingMode = PickingMode.Position,
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    paddingLeft = 0,
                    paddingRight = 0,
                    paddingTop = 0,
                    paddingBottom = 0,
                    marginLeft = 0,
                    marginRight = 0,
                    marginTop = 0,
                    marginBottom = 0
                }
            };

            SetupInputReader(_root);
            SetupRegistryFocus();

            var versionTracker = new WindowConfigurationVersion();
            _builder = new WindowConfigurationBuilder(Controllers, Settings, InspectionTypeId, versionTracker);

            EdixorBootstrapper.Bootstrap(
                controllers: Controllers,
                settings: Settings,
                root: _root,
                container: Container,
                configure: () => Configure(_builder)
            );

            _root.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                var panelRoot = _root;
                while (panelRoot.parent != null)
                    panelRoot = panelRoot.parent;

                var inspector = panelRoot.Q<VisualElement>("Demonstration Edixor Inspector (Demonstration Edixor Inspector Default)Inspector");
                var footer = panelRoot.Q<VisualElement>("Demonstration Edixor Inspector (Demonstration Edixor Inspector Default)Footer");
                if (inspector != null)
                {
                    inspector.style.paddingLeft = 0;
                    inspector.style.paddingRight = 0;
                    inspector.style.paddingTop = 0;
                    inspector.style.paddingBottom = 0;
                }
                if (footer != null)
                    footer.style.width = Length.Percent(0);
            });

            Controllers.InitOptions(_root, Container, Settings);
            InitializeInspectorTab();
            Controllers.Initialize();

            FileDragHandler.RegisterDragHandlers(_root, OnTabAddedFromDrag);
            _root.RegisterCallback<KeyDownEvent>(_ => Controllers.OnKeys());

            EditorApplication.update += CheckFocus;

            return _root;
        }

        private void SetupInputReader(VisualElement root)
        {
            var reader = new InputReader(root);
            Container.RegisterNamed<InputReader>(ServiceNameKeys.Edixor_Input + InspectionTypeId, reader);

            reader.OnKeyDown += key => Controllers.OnKeys();
            reader.OnKeyUp += key => Controllers.OnKeys();

            EditorApplication.delayCall += () => root.Focus();
        }

        private void SetupRegistryFocus()
        {
            Registry.SetCorrectItem(InspectionTypeId);
            EditorApplication.delayCall += () =>
            {
                Registry.SetCorrectItem(InspectionTypeId);
            };
        }

        private void RegisterServices()
        {
            Container.RegisterNamed<IClosable>(ServiceNameKeys.Edixor_Close + InspectionTypeId, this);
            Container.RegisterNamed<IRestartable>(ServiceNameKeys.Edixor_Restart + InspectionTypeId, this);
        }

        private void CheckFocus()
        {
            var window = EditorWindow.focusedWindow;
            if (window != null && window.GetType().Name.Contains("Inspector"))
            {
                if (!_hadFocus)
                {
                    _hadFocus = true;
                    OnInspectorFocus();
                }
            }
            else
            {
                _hadFocus = false;
            }
        }

        private void OnInspectorFocus()
        {
            _root.Focus();
            Registry.SetCorrectItem(InspectionTypeId);
        }

        protected virtual void Configure(WindowConfigurationBuilder builder)
        {
            builder
                .HotKey("Exit")
                .HotKey("Restart")
                .Function("HotKey")
                .Function("Setting")
                .Function("Restart")
                .Status("Version")
                .Status("Key Combination")
                .BasicTab("ExtensionInspectTab")
                .Layout("Standard")
                .Style("Unity");
        }

        protected virtual void OnMainTab(VisualElement mainTab)
        {
            mainTab.Add(new IMGUIContainer(() => DrawDefaultInspector()));
        }

        private void InitializeInspectorTab()
        {
            var mainContent = new VisualElement
            {
                name = "EdixorInspectorTarget",
                style = { flexGrow = 0 }
            };

            OnMainTab(mainContent);

            if (_builder.BasicTabInstance is IInitializableContent initializer)
                initializer.InitializeContent(mainContent);

            var EdixorTab = Controllers.GetLogicFor(_builder.BasicTabInstance);
            EdixorTab?.Initialize(mainContent, Container);

            var fillElement = new VisualElement
            {
                name = "FillSpacer",
                style =
                {
                    backgroundColor = new Color(1f, 0f, 0f, 0.05f), 
                    height = 0,
                    flexShrink = 0
                }
            };

            var container = new VisualElement
            {
                name = "EdixorContentContainer",
                style = { flexDirection = FlexDirection.Column, flexGrow = 1 }
            };

            container.Add(mainContent);
            container.Add(fillElement);
            _root.Add(container);

            _root.schedule.Execute(() =>
            {
                float rootHeight = _root.resolvedStyle.height;
                float contentHeight = mainContent.resolvedStyle.height;

                float remaining = rootHeight - contentHeight;
                if (remaining > 0)
                {
                    fillElement.style.height = remaining;
                }
                else
                {
                    fillElement.style.height = 0;
                }
            }).ExecuteLater(200);
        }



        private void OnTabAddedFromDrag(string filePath, string className)
        {
            FileDragHandler.AddTabFromFile(filePath, className, (file, tabType) =>
            {
                var tab = (TabData)Activator.CreateInstance(tabType);
                Controllers.AddTab(tab, false, true);
            });
        }

        public void RestartWindow()
        {
            if (_root == null) return;

            _root.Clear();
            InitializeInspectorTab();
            FileDragHandler.RegisterDragHandlers(_root, OnTabAddedFromDrag);
            _root.RegisterCallback<KeyDownEvent>(_ => Controllers.OnKeys());
        }

        public void CloseEdixor()
        {
            _root?.Clear();
            Controllers.OnWindowClose();
        }

        private void OnDisable()
        {
            CurrentInspector = null;
            Controllers?.OnWindowClose();
        }

        private string ComputeDeterministicId()
        {
            string key = GetType().AssemblyQualifiedName;
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
            return new Guid(hash).ToString();
        }

        private string ComputeName() => GetType().Name;

        private WindowStateSetting CreateWindowState()
        {
            var state = new WindowStateSetting(WindowName);
            state.SetRootElement(_root);
            return state;
        }

        private IControllersFacade InitializeControllers()
        {
            IControllersFacade facade;
            string controllerKey = ServiceNameKeys.EdixorControllers + InspectionTypeId;

            if (!Container.IsRegisteredNamed<IControllersFacade>(controllerKey))
            {
                IUIController ui = new UIController(Container);
                ITabController tabs = new TabController(Container);
                IFunctionController func = new FunctionController(ui, Container);
                IStatusController status = new StatusController(ui, Container);
                IHotKeyController hkey = new HotKeyController(Container, Settings);
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

                Container.RegisterNamed<IUIController>(ServiceNameKeys.UIController + InspectionTypeId, ui);
                Container.RegisterNamed<ITabController>(ServiceNameKeys.TabController + InspectionTypeId, tabs);
                Container.RegisterNamed<IFunctionController>(ServiceNameKeys.FunctionController + InspectionTypeId, func);
                Container.RegisterNamed<IStatusController>(ServiceNameKeys.StatusController + InspectionTypeId, status);
                Container.RegisterNamed<IHotKeyController>(ServiceNameKeys.HotKeyController + InspectionTypeId, hkey);
                Container.RegisterNamed<IControllersFacade>(controllerKey, facade);
            }
            else
            {
                facade = Container.ResolveNamed<IControllersFacade>(controllerKey);
            }

            return facade;
        }

        private ISettingsFacade InitializeSetting()
        {
            var settingsKey = ServiceNameKeys.EdixorSettings + InspectionTypeId;

            if (!Container.IsRegisteredNamed<ISettingsFacade>(settingsKey))
            {
                var settings = new SettingsFacade(WindowName, InspectionTypeId, Container);
                Container.RegisterNamed<ISettingsFacade>(settingsKey, settings);
                return settings;
            }

            return Container.ResolveNamed<ISettingsFacade>(settingsKey);
        }
    }
}
