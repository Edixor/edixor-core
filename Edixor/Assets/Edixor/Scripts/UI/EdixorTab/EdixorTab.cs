using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public abstract class EdixorTab
{
    [SerializeField] protected string tabName;
    [SerializeField] protected int openCount = 0;
    [SerializeField] private string pathUxml;
    [SerializeField] private string pathUss;
    private EdixorHotKeys _hotKeyController;
    private EdixorHotKeys hotKeyController
    {
        get
        {
            if (_hotKeyController == null)
            {
                _hotKeyController = container.ResolveNamed<EdixorHotKeys>(ServiceNames.EdixorHotKeys_EdixorWindow);
            }
            return _hotKeyController;
        }
    }

    protected VisualElement ParentContainer;
    protected VisualElement root;
    protected DIContainer container;

    public string Title => tabName;
    public string PathUxml => pathUxml;
    public string PathUss => pathUss;
    public int OpenCount => openCount;

    public event Action<HotKeyTabInfo> OnHotKeyAdded;
    private Action childAwake;
    private Action childStart;
    private Action childOnEnable;
    private Action childOnDisable;
    private Action childOnDestroy;
    private Action childUpdate;

    private bool lifecycleDelegatesSet = false;

    public void Initialize(VisualElement ParentContainer, DIContainer container)
    {
        this.ParentContainer = ParentContainer;
        this.container = container;

        SetupLifecycleDelegates();
    }

    protected void Initialize(string tabName, string pathUxml = null, string pathUss = null)
    {
        this.tabName = tabName;
        if (!string.IsNullOrEmpty(pathUxml))
        {
            this.pathUxml = pathUxml;
            LoadUxml(pathUxml);
        }
        if (!string.IsNullOrEmpty(pathUss))
        {
            this.pathUss = pathUss;
            LoadUss(pathUss);
        }

        SetupLifecycleDelegates();
    }

    public static void ShowTab<T>() where T : EdixorTab, new()
    {
        DIContainer container = LoadOrCreateContainer();

        EdixorWindow window;
        if (EdixorWindow.CurrentWindow == null)
        {
            window = EditorWindow.GetWindow<EdixorWindow>("EdixorWindow");
        }
        else
        {
            window = EdixorWindow.CurrentWindow;
            window.Focus();
        }

        container.ResolveNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow)
            .AddTab(new T(), saveState: false, autoSwitch: true);
    }

    protected static DIContainer LoadOrCreateContainer()
    {
        string containerPath = PathResolver.ResolvePath("Assets/Edixor/DIContainer.asset");
        var container = AssetDatabase.LoadAssetAtPath<DIContainer>(containerPath);

        if (container == null)
        {
            container = ScriptableObject.CreateInstance<DIContainer>();
            AssetDatabase.CreateAsset(container, containerPath);
            AssetDatabase.SaveAssets();
        }

        return container;
    }


    private void SetupLifecycleDelegates()
    {
        if (lifecycleDelegatesSet)
            return;

        Type childType = this.GetType();

        MethodInfo mi = childType.GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childAwake = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childStart = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childOnEnable = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childOnDisable = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childOnDestroy = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("UpdateGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (IsValidLifecycleMethod(mi))
            childUpdate = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        lifecycleDelegatesSet = true;
    }

    private bool IsValidLifecycleMethod(MethodInfo mi)
    {
        return mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(void);
    }

    public void InvokeAwake()
    {
        LoadUxml(pathUxml);

        if (!lifecycleDelegatesSet)
            SetupLifecycleDelegates();
        childAwake?.Invoke();
    }

    public void InvokeStart()
    {
        childStart?.Invoke();
    }

    public void InvokeOnEnable()
    {
        childOnEnable?.Invoke();
    }

    public void InvokeOnDisable()
    {
        childOnDisable?.Invoke();
    }

    public void InvokeOnDestroy()
    {
        childOnDestroy?.Invoke();
    }

    public void InvokeUpdateGUI()
    {
        childUpdate?.Invoke();
    }

    protected void LoadUxml(string pathUxml)
    {
        // Если путь пустой или null, подставляем дефолтный
        if (string.IsNullOrEmpty(pathUxml))
        {
            pathUxml = "Assets/Edixor/Scripts/UI/EdixorTab/default.uxml";
            Debug.Log($"Путь к UXML не указан. Используем дефолтный: {pathUxml}");
        }
        else
        {
            this.pathUxml = pathUxml;
        }

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(pathUxml);
        if (visualTree == null)
        {
            Debug.LogError($"UI файл не найден по пути: {pathUxml}");
            return;
        }

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("ParentContainer равен null. Невозможно загрузить UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);

        Debug.Log($"UI успешно загружен из {pathUxml}");
    }

    protected void LoadUss(string pathUss)
    {
        if (!string.IsNullOrEmpty(pathUss))
        {
            this.pathUss = pathUss;
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(pathUss);
            if (styleSheet == null)
            {
                Debug.LogError("StyleSheet file not found at " + pathUss);
                return;
            }
            root.styleSheets.Add(styleSheet);
        }
    }

    protected void LoadHotKey(string path, Action action)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Путь к горячей клавише пустой или null. Невозможно добавить горячую клавишу.");
            return;
        }

        KeyActionData keyActionData = AssetDatabase.LoadAssetAtPath<KeyActionData>(path);
        if (keyActionData == null)
        {
            Debug.LogError($"KeyActionData не найден по пути: {path}");
            return;
        }

        KeyActionLogic logic = new CustomKeyAction(container, action);
        hotKeyController.AddKey(new KeyAction(keyActionData, logic));

        container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting).AddHotKeyToDictionary(Title, new[] { keyActionData });

        OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
        Debug.Log($"Горячая клавиша '{keyActionData.Name}' успешно добавлена.");
    }

    protected void LoadHotKeys(string[] paths, Action[] actions)
    {
        Debug.Log("SSSSSSSSSSSSSS1");
        if (paths == null || paths.Length == 0)
        {
            Debug.LogError("Path array is null or empty. Cannot load hotkeys.");
            return;
        }

        List<KeyActionData> loadedKeys = new List<KeyActionData>();

        for (int i = 0; i < paths.Length; i++)
        {
            if (string.IsNullOrEmpty(paths[i]))
            {
                Debug.LogWarning("Один из путей пустой. Пропускаем.");
                continue;
            }

            KeyActionData keyActionData = AssetDatabase.LoadAssetAtPath<KeyActionData>(paths[i]);
            if (keyActionData == null)
            {
                Debug.LogError($"KeyActionData not found at {paths[i]}");
                continue;
            }

            KeyActionLogic logic = new CustomKeyAction(container, actions[i]);
            hotKeyController.AddKey(new KeyAction(keyActionData, logic));
            loadedKeys.Add(keyActionData);
        }

        if (loadedKeys.Count > 0)
        {
            container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting).AddHotKeyToDictionary(Title, loadedKeys.ToArray());
            OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
        }
        else
        {
            Debug.LogWarning("Ни одной горячей клавиши не было загружено.");
        }
    }

    public void DeleteUI()
    {
        if (root != null)
        {
            root = null;
        }

        InvokeOnDestroy();
    }
}
