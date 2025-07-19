using UnityEditor;
using UnityEngine;
using System.IO;

using ExTools.Settings;
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
        if (_container == null)
        {
            _container = EdixorObjectLocator.LoadObject<DIContainer>("Servers/DIContainer.asset");
            InitializeServices(_container);
        }

        InitializeSettings(_container);

        OtherEdixorSettingAsset.UpdateSetting -= OnSettingsUpdated;
        OtherEdixorSettingAsset.UpdateSetting += OnSettingsUpdated;

        ExDebug.EndGroup();
    }

    
    private static void OnSettingsUpdated()
    {
        ExDebug.BeginGroup("DI: edixor settings updated");
        ExDebug.Log("DI: Settings changed! Reinitializing settings only.");
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

        container.RegisterNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting, new LayoutSetting());
        container.RegisterNamed<StyleSetting>(ServiceNameKeys.StyleSetting, new StyleSetting());
        container.RegisterNamed<OtherEdixorSetting>(ServiceNameKeys.OtherEdixorSetting, new OtherEdixorSetting());

        EdixorRegistrySetting registrySetting = new EdixorRegistrySetting();
        registrySetting.SetColdStart(true);
        container.RegisterNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting, registrySetting);

        container.RegisterSingleton<HotkeyCaptureHandler, HotkeyCaptureHandler>();

        container.RegisterNamed<IHotkeyCaptureHandler>(ServiceNameKeys.HotkeyCaptureHandler, new HotkeyCaptureHandler());

        container.Register<IExtensionIndexProviderFactory, GitHubIndexProviderFactory>();
        container.RegisterNamed<IExtensionInstaller>(ServiceNameKeys.GitHubExtensionInstaller, new GitHubExtensionInstaller());

        container.Register<ServiceNameResolver>(new ServiceNameResolver(container));

        TabRegistrationBootstrap.RegisterTabs();
        
        ExDebug.Log("Services registered successfully");
    }

    private static void InitializeSettings(DIContainer container)
    {
        if (container == null)
        {
            ExDebug.LogError("DI: DIContainer is null, initialization of settings interrupted");
            return;
        }

        ExDebug.InitSetting(container.ResolveNamed<OtherEdixorSetting>(ServiceNameKeys.OtherEdixorSetting).IsConsoleLoggingEnabled());

        ExDebug.Log("Settings loaded successfully");
    }
}
