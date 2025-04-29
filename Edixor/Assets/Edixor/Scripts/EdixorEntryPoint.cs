using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class EdixorEntryPoint
{
    private static DIContainer _container;

    static EdixorEntryPoint()
    {
        Debug.Log("Проект открыт! Запускаем свою логику.");

        string containerPath = PathResolver.ResolvePath("Assets/Edixor/DIContainer.asset");
        LoadOrCreateContainer(containerPath);

        InitializeServices(_container);
    }

    private static void LoadOrCreateContainer(string path)
    {
        if (File.Exists(path))
        {
            _container = AssetDatabase.LoadAssetAtPath<DIContainer>(path);
        }
        else
        {
            _container = ScriptableObject.CreateInstance<DIContainer>();
            AssetDatabase.CreateAsset(_container, path);
            AssetDatabase.SaveAssets();
        }
    }

    private static void InitializeServices(DIContainer container)
    {
        if (container == null)
        {
            Debug.LogError("DI контейнер не загружен. Инициализация сервисов прервана.");
            return;
        }

        Register register = new Register();

        container.Register<IRegister>(register);

        container.RegisterNamed<LayoutSetting>(ServiceNames.LayoutSetting, new LayoutSetting());
        container.RegisterNamed<StyleService>(ServiceNames.StyleSetting, new StyleService(register));
        container.RegisterNamed<WindowStateService>(ServiceNames.WindowStateSetting, new WindowStateService());
        container.RegisterNamed<FunctionSetting>(ServiceNames.FunctionSetting, new FunctionSetting(container));
        container.RegisterNamed<TabService>(ServiceNames.TabSetting, new TabService(register));
        container.RegisterNamed<HotKeySetting>(ServiceNames.HotKeySetting, new HotKeySetting());

        container.RegisterNamed<FunctionLogic>("RestartFunc", new Restart());
        container.RegisterNamed<FunctionLogic>("Close", new Close());
        container.RegisterNamed<FunctionLogic>("HotKey", new HotKeys());
        container.RegisterNamed<FunctionLogic>("Setting", new Setting());

        container.RegisterSingleton<HotkeyCaptureHandler, HotkeyCaptureHandler>();

        container.Register<IUIController>(new UIController(container));
        container.Register<ITabController>(new TabController(container));
        container.Register<IFunctionController>(new FunctionController(container));
        container.Register<IHotKeyController>(new HotKeyController(container));

        container.Register<IFactoryHotKey>(new FactoryHotKey(container.ResolveNamed<HotKeySetting>(ServiceNames.HotKeySetting)));
        container.Register<IFactoryFunction>(new FactoryFunction(container.ResolveNamed<FunctionSetting>(ServiceNames.FunctionSetting)));

        container.RegisterNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_Edixor,
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
        
        Debug.Log("Сервисы успешно зарегистрированы.");
    }
}
