using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using ExTools;
using System.Linq;

[Serializable]
public abstract class EdixorTab
{
    [SerializeField] protected string tabName;
    [SerializeField] protected int openCount;
    [SerializeField] private string pathUxml;
    [SerializeField] private string pathUss;

    [NonSerialized] private IEdixorInterfaceFacade _setting;
    [NonSerialized] protected DIContainer container;
    [NonSerialized] protected VisualElement ParentContainer;
    [NonSerialized] protected VisualElement root;
    
    // Lifecycle delegates are not serialized
    [NonSerialized] private Action childAwake;
    [NonSerialized] private Action childStart;
    [NonSerialized] private Action childOnEnable;
    [NonSerialized] private Action childOnDisable;
    [NonSerialized] private Action childOnDestroy;
    [NonSerialized] private Action childUpdate;
    [NonSerialized] private bool lifecycleDelegatesSet = false;

    public string Title => tabName;
    public string PathUxml => pathUxml;
    public string PathUss => pathUss;
    public int OpenCount => openCount;

    public event Action<HotKeyTabInfo> OnHotKeyAdded;

    protected IEdixorInterfaceFacade setting
    {
        get
        {
            if (_setting == null)
                _setting = container.ResolveNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorWindow);
            return _setting;
        }
    }

    public void Initialize(VisualElement parent, DIContainer cont)
    {
        ParentContainer = parent;
        container = cont;
        SetupLifecycleDelegates();
    }

    public static void ShowTab<T>() where T : EdixorTab, new()
    {
        DIContainer cnt = LoadOrCreateContainer();
        EdixorWindow wnd = EdixorWindow.CurrentWindow ?? EditorWindow.GetWindow<EdixorWindow>("EdixorWindow");
        T tab = new T();
        tab.container = cnt;
        tab.setting.AddTab(tab);
    }

    protected void Option(string title, string uxml, string uss) {
        tabName = title;
        LoadUxml(uxml);
        LoadUss(uss);
    }

    protected static DIContainer LoadOrCreateContainer()
    {
        string containerPath = PathResolver.ResolvePath("Assets/Edixor/Editor/Core/Services/DIContainer.asset");
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
        if (lifecycleDelegatesSet) return;
        Type childType = GetType();
        childAwake     = CreateDelegateIfExists(childType, "Awake");
        childStart     = CreateDelegateIfExists(childType, "Start");
        childOnEnable  = CreateDelegateIfExists(childType, "OnEnable");
        childOnDisable = CreateDelegateIfExists(childType, "OnDisable");
        childOnDestroy = CreateDelegateIfExists(childType, "OnDestroy");
        childUpdate    = CreateDelegateIfExists(childType, "UpdateGUI");
        lifecycleDelegatesSet = true;
    }

    private Action CreateDelegateIfExists(Type type, string methodName)
    {
        MethodInfo mi = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(void))
            return (Action)Delegate.CreateDelegate(typeof(Action), this, mi);
        return null;
    }

    public void InvokeAwake()    { LoadUxml(pathUxml); childAwake?.Invoke(); }
    public void InvokeStart()    { childStart?.Invoke(); }
    public void InvokeOnEnable(){ 
        childOnEnable?.Invoke(); 
        EdixorParameters p = (EdixorParameters)container
            .ResolveNamed<StyleSetting>(ServiceNames.StyleSetting)
            .GetCorrectItem()
            .AssetParameters[0];
        InitStyleTab(p);
    }
    public void InvokeOnDisable(){ childOnDisable?.Invoke(); }
    public void InvokeOnDestroy(){ childOnDestroy?.Invoke(); }
    public void InvokeUpdateGUI(){ childUpdate?.Invoke(); }

    public void InitStyleTab(EdixorParameters parameters)
    {
        if (parameters == null)
        {
            Debug.LogError("EdixorParameters is null. Cannot apply styles.");
            return;
        }
        if (root == null)
        {
            Debug.LogError("Root VisualElement is null. Cannot apply styles.");
            return;
        }

        // 0) Прокинем класс E-Scroll на все внутренние scrollers всех ScrollView
        foreach (var scroll in root.Query<ScrollView>().ToList())
        {
            scroll.horizontalScroller.AddToClassList("E-Scroll");
            scroll.verticalScroller  .AddToClassList("E-Scroll");
        }

        // 1) Применение обычных стилей
        foreach (var styleEntry in parameters.Styles)
        {
            Debug.Log($"Found {styleEntry.Name} style entry");
            var elements = root.Query<VisualElement>()
                            .Class(styleEntry.Name)
                            .ToList();
            foreach (var element in elements)
                styleEntry.Style.ApplyWithStates(element);
        }

        // 2) Применение стилей для Scroll/Slider по классу E-Scroll
        foreach (var scrollEntry in parameters.ScrollStyles)
        {
            var elements = root.Query<VisualElement>()
                            .Class(scrollEntry.Name)  // здесь scrollEntry.Name == "E-Scroll"
                            .ToList();

            Debug.Log($"Found {elements.Count} elements with class '{scrollEntry.Name}'");
            foreach (var element in elements)
            {
                Debug.Log($"  → Applying scroll-style to '{element.name}' ({element.GetType().Name})");
                scrollEntry.Style.ApplyScrollStyleWithStates(element);
            }
        }
    }



    protected void LoadUxml(string path = null)
    {
        string uxmlPath = string.IsNullOrEmpty(path) || path == "auto"
            ? $"Tabs/{GetType().Name}/{GetType().Name}.uxml"
            : path;

        var visualTree = EdixorObjectLocator.LoadObject<VisualTreeAsset>(uxmlPath);
        if (visualTree == null)
        {
            //ExDebug.LogWarning($"UI файл не найден по пути: {uxmlPath}. Пытаюсь загрузить Default…");

            uxmlPath = "Tabs/Default/Default.uxml";
            visualTree = EdixorObjectLocator.LoadObject<VisualTreeAsset>(uxmlPath);
            if (visualTree == null)
            {
                //ExDebug.LogError($"UI-файл Default не найден по пути: {uxmlPath}. Отмена загрузки.");
                return;
            }
        }

        pathUxml = uxmlPath;
        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            //ExDebug.LogError("ParentContainer равен null. Невозможно загрузить UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);
    }



    protected void LoadUss(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        string ussPath = path == "auto"
            ? $"Tabs/{GetType().Name}/{GetType().Name}.uss"
            : path;
        var sheet = EdixorObjectLocator.LoadObject<StyleSheet>(ussPath);
        if (sheet == null)
        {
            ExDebug.LogError($"StyleSheet не найден по пути: {ussPath}");
            return;
        }

        pathUss = ussPath;
        root.styleSheets.Add(sheet);
    }

    protected void LoadHotKey(string path, Action action = null, string title = null)
    {
        setting.LoadHotKey(path, title ?? tabName, action);
        OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
    }

    protected void ChangeStyle(int index)
    {
        container.ResolveNamed<StyleSetting>(ServiceNames.StyleSetting).GetCorrectItem();
    }

    protected void ChangeLayout(int index)
    {
        container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting).UpdateIndex(index);
    }

    public void DeleteUI()
    {
        root = null;
        InvokeOnDestroy();
    }
}

