using UnityEditor;
using UnityEngine;
using System.IO;
using ExTools;

[InitializeOnLoad]
public static class EdixorEntryPoint
{
    private static DIContainer _container;

    static EdixorEntryPoint()
    {
        ExDebug.BeginGroup("DI: edixor is open");
        ExDebug.Log("DI: The edixor is open! Let's launch our logic");
        Initialize();
    }

    public static void Reinitialize()
    {
        ExDebug.BeginGroup("DI: edixor is restarting");
        ExDebug.Log("DI: The edixor is restarting! Let's launch our logic");
        Initialize();
    }

    private static void Initialize()
    {
        OtherEdixorSettingAsset.UpdateSetting += () =>
        {
            ExDebug.Log("DI: The edixor is updating! Let's launch our logic");
            InitializeSettings(_container);
        }; 
        _container = EdixorObjectLocator.LoadObject<DIContainer>("Servers/DIContainer.asset");
        InitializeServices(_container);
        InitializeSettings(_container);
        ExDebug.EndGroup();
    }

    private static void InitializeServices(DIContainer container)
    {
        if (container == null)
        {
            ExDebug.LogError("DI: DIContainer is null, initialization of services interrupted");
            return;
        }

        Register register = new Register();

        container.Register<IRegister>(register);


        container.RegisterNamed<LayoutSetting>(ServiceNames.LayoutSetting, new LayoutSetting());
        container.RegisterNamed<StyleSetting>(ServiceNames.StyleSetting, new StyleSetting());
        container.RegisterNamed<WindowStateSetting>(ServiceNames.WindowStateSetting, new WindowStateSetting());
        container.RegisterNamed<FunctionSetting>(ServiceNames.FunctionSetting, new FunctionSetting(container));
        container.RegisterNamed<TabSetting>(ServiceNames.TabSetting, new TabSetting());
        container.RegisterNamed<HotKeySetting>(ServiceNames.HotKeySetting, new HotKeySetting(container));
        container.RegisterNamed<OtherEdixorSetting>(ServiceNames.OtherEdixorSetting, new OtherEdixorSetting());

        container.RegisterNamed<FunctionLogic>("RestartFunc", new Restart());
        container.RegisterNamed<FunctionLogic>("Close", new Close());
        container.RegisterNamed<FunctionLogic>("HotKey", new HotKeys());
        container.RegisterNamed<FunctionLogic>("Setting", new Setting());

        container.RegisterSingleton<HotkeyCaptureHandler, HotkeyCaptureHandler>();

        container.Register<IUIController>(new UIController(container));
        container.Register<ITabController>(new TabController(container));
        container.Register<IFunctionController>(new FunctionController(container.Resolve<IUIController>(), container));
        container.Register<IHotKeyController>(new HotKeyController(container));

        container.Register<IFactoryHotKey>(new FactoryHotKey(container.ResolveNamed<HotKeySetting>(ServiceNames.HotKeySetting)));
        container.Register<IFactoryFunction>(new FactoryFunction(container.ResolveNamed<FunctionSetting>(ServiceNames.FunctionSetting)));

        container.RegisterNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorInspector,
        new EdixorInterfaceFacade(
            container.Resolve<ITabController>(),
            container.Resolve<IUIController>(),
            container.Resolve<IFunctionController>(),
            container.Resolve<IHotKeyController>(),
            container.Resolve<IFactoryHotKey>(),
            container.Resolve<IFactoryFunction>()
            ));
            
        container.RegisterNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorWindow,
        new EdixorInterfaceFacade(
            container.Resolve<ITabController>(),
            container.Resolve<IUIController>(),
            container.Resolve<IFunctionController>(),
            container.Resolve<IHotKeyController>(),
            container.Resolve<IFactoryHotKey>(),
            container.Resolve<IFactoryFunction>()
            ));

        container.RegisterNamed<IHotkeyCaptureHandler>(ServiceNames.IHotkeyCaptureHandler, new HotkeyCaptureHandler());

        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Restart, new KeyRestart());
        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Minimizable, new Minimizable());
        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Exit, new Exit());

        container.RegisterNamed<IExtensionIndexProvider>(ServiceNames.GitHubExtensionIndexProvider, new GitHubExtensionIndexProvider());
        container.RegisterNamed<IExtensionInstaller>(ServiceNames.GitHubExtensionInstaller, new GitHubExtensionInstaller());
        
        ExDebug.Log("Services registered successfully");
    }

    private static void InitializeSettings(DIContainer container)
    {
        if (container == null)
        {
            ExDebug.LogError("DI: DIContainer is null, initialization of settings interrupted");
            return;
        }

        ExDebug.InitSetting(container.ResolveNamed<OtherEdixorSetting>(ServiceNames.OtherEdixorSetting).IsConsoleLoggingEnabled());

        ExDebug.Log("Settings loaded successfully");
    }
}
