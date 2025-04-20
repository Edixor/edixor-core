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

        Factory factory = new Factory();
        Register register = new Register();

        container.Register<IFactory>(factory);
        container.Register<IRegister>(register);

        container.RegisterNamed<LayoutService>(ServiceNames.LayoutSetting, new LayoutService(register));
        container.RegisterNamed<StyleService>(ServiceNames.StyleSetting, new StyleService(register));
        container.RegisterNamed<WindowStateService>(ServiceNames.WindowStateSetting, new WindowStateService());
        container.RegisterNamed<FunctionService>(ServiceNames.FunctionSetting, new FunctionService());
        container.RegisterNamed<TabService>(ServiceNames.TabSetting, new TabService(register));
        container.RegisterNamed<HotKeyService>(ServiceNames.HotKeySetting, new HotKeyService());

        container.RegisterSingleton<HotkeyCaptureHandler, HotkeyCaptureHandler>();

        container.Register<IUIController>(new UIController());
        container.Register<ITabController>(new TabController(container));
        container.Register<IFunctionController>(new FunctionController());
        container.Register<IHotKeyController>(new HotKeyController(container));

        container.RegisterNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_Edixor,
        new EdixorInterfaceFacade(
            container.Resolve<ITabController>(),
            container.Resolve<IUIController>(),
            container.Resolve<IFunctionController>(),
            container.Resolve<IHotKeyController>()));
            
        container.RegisterNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorWindow,
        new EdixorInterfaceFacade(
            container.Resolve<ITabController>(),
            container.Resolve<IUIController>(),
            container.Resolve<IFunctionController>(),
            container.Resolve<IHotKeyController>()));

        container.RegisterNamed<IHotkeyCaptureHandler>(ServiceNames.IHotkeyCaptureHandler, new HotkeyCaptureHandler());

        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Restart, new KeyRestart());
        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Minimizable, new Minimizable());
        container.RegisterNamed<KeyActionLogic>(HotKeyNames.Exit, new Exit());
        
        Debug.Log("Сервисы успешно зарегистрированы.");
    }
}
