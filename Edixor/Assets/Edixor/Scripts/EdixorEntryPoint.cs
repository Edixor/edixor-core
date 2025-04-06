using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class ProjectStartupHandler
{
    private static DIContainer _container;

    static ProjectStartupHandler()
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

        container.RegisterNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_Edixor, new EdixorUIManager(container));
        container.RegisterNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow, new EdixorUIManager(container));

        container.RegisterNamed<EdixorHotKeys>(ServiceNames.EdixorHotKeys_Edixor, new EdixorHotKeys(container));
        container.RegisterNamed<EdixorHotKeys>(ServiceNames.EdixorHotKeys_EdixorWindow, new EdixorHotKeys(container));
        
        Debug.Log("Сервисы успешно зарегистрированы.");
    }
}
