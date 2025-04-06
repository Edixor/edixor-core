using UnityEditor;
using UnityEngine;

public static class DIProvider
{
    private static DIContainer _container;
    private static bool _isInitialized = false;

    public static void Initialize()
    {
        if (_isInitialized) return;

        _container = AssetDatabase.LoadAssetAtPath<DIContainer>("Assets/PathToYourAsset/NewDIContainer.asset");
        if (_container == null)
        {
            string[] guids = AssetDatabase.FindAssets("t:DIContainerSO");
            if (guids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _container = AssetDatabase.LoadAssetAtPath<DIContainer>(assetPath);
            }
        }

        if (_container == null)
        {
            Debug.LogError("DIContainer asset not found!");
            return;
        }

        _isInitialized = true;
        Debug.Log("DIContainer успешно загружен.");
    }

    public static void EnsureInitialized()
    {
        if (!_isInitialized)
        {
            Debug.LogError("DI контейнер ещё не загружен! Не забудь вызвать DIProvider.Initialize() перед использованием.");
        }
    }

    public static DIContainer Container
    {
        get
        {
            EnsureInitialized();
            return _container;
        }
    }

    public static T Resolve<T>()
    {
        EnsureInitialized();
        return Container.Resolve<T>();
    }

    public static void Register<T>(T service)
    {
        EnsureInitialized();
        Container.Register(service);
    }
}
