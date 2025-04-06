using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;

[Serializable]
public abstract class EdixorTab
{
    [SerializeField] private string tabName;
    [SerializeField] private string pathUxml;
    [SerializeField] private string pathUss;
    [SerializeField] private int openCount = 0;

    protected VisualElement ParentContainer;
    protected VisualElement root;
    protected DIContainer container;

    public string Title => tabName;
    public string PathUxml => pathUxml;
    public string PathUss => pathUss;
    public int OpenCount => openCount;

    // Делегаты для методов жизненного цикла, найденных в дочернем классе.
    private Action childAwake;
    private Action childStart;
    private Action childOnEnable;
    private Action childOnDisable;
    private Action childOnDestroy;
    private Action childOnUI;

    /// <summary>
    /// Инициализирует вкладку, устанавливая контейнер, DI-контейнер, имя и пути к UI-файлам.
    /// Также выполняется поиск методов жизненного цикла в дочернем классе.
    /// </summary>
    public void Initialize(VisualElement ParentContainer, DIContainer container, string tabName, string pathUxml = null, string pathUss = null)
    {
        this.ParentContainer = ParentContainer;
        this.container = container;
        this.tabName = tabName;
        this.pathUxml = pathUxml;
        this.pathUss = pathUss;

        SetupLifecycleDelegates();
        LoadUI();
    }

    /// <summary>
    /// Выполняет поиск методов жизненного цикла (Awake, Start, OnEnable, OnDisable, OnDestroy, OnUI)
    /// в дочернем классе посредством рефлексии и сохраняет их в делегаты.
    /// </summary>
    private void SetupLifecycleDelegates()
    {
        Type childType = this.GetType();

        MethodInfo mi = childType.GetMethod("Awake", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childAwake = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childStart = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childOnEnable = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childOnDisable = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childOnDestroy = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);

        mi = childType.GetMethod("OnUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
        if (IsValidLifecycleMethod(mi))
            childOnUI = (Action)Delegate.CreateDelegate(typeof(Action), this, mi);
    }

    private bool IsValidLifecycleMethod(MethodInfo mi)
    {
        return mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(void);
    }

    /// <summary>
    /// Вызывает метод Awake, определённый в дочернем классе.
    /// </summary>
    public void InvokeAwake()
    {
        childAwake?.Invoke();
    }

    /// <summary>
    /// Вызывает метод Start, определённый в дочернем классе.
    /// </summary>
    public void InvokeStart()
    {
        childStart?.Invoke();
    }

    /// <summary>
    /// Вызывает метод OnEnable, определённый в дочернем классе.
    /// </summary>
    public void InvokeOnEnable()
    {
        childOnEnable?.Invoke();
    }

    /// <summary>
    /// Вызывает метод OnDisable, определённый в дочернем классе.
    /// </summary>
    public void InvokeOnDisable()
    {
        childOnDisable?.Invoke();
    }

    /// <summary>
    /// Вызывает метод OnDestroy, определённый в дочернем классе.
    /// </summary>
    public void InvokeOnDestroy()
    {
        childOnDestroy?.Invoke();
    }

    /// <summary>
    /// Вызывает метод OnUI, определённый в дочернем классе.
    /// </summary>
    public void InvokeOnUI()
    {
        childOnUI?.Invoke();
    }

    /// <summary>
    /// Загружает UI из UXML и применяет USS.
    /// </summary>
    protected void LoadUI()
    {
        if (string.IsNullOrEmpty(pathUxml))
        {
            Debug.LogWarning("UXML path is null or empty. Skipping UI loading.");
            return;
        }

        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(pathUxml);
        if (visualTree == null)
        {
            Debug.LogError("UI file not found at " + pathUxml);
            return;
        }

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("ParentContainer is null. Cannot load UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);

        if (!string.IsNullOrEmpty(pathUss))
        {
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(pathUss);
            if (styleSheet == null)
            {
                Debug.LogError("StyleSheet file not found at " + pathUss);
                return;
            }
            root.styleSheets.Add(styleSheet);
        }
    }

    /// <summary>
    /// Удаляет UI и вызывает метод OnDestroy.
    /// </summary>
    public void DeleteUI()
    {
        if (root != null)
        {
            root = null;
        }

        InvokeOnDestroy();
    }
}
