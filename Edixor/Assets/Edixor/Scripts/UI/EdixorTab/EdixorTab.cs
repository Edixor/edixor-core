using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

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
                _hotKeyController = container.ResolveNamed<EdixorHotKeys>(ServiceNames.EdixorHotKeys_EdixorWindow);
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
        cnt.ResolveNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow)
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
    public void InvokeOnEnable(){ childOnEnable?.Invoke(); }
    public void InvokeOnDisable(){ childOnDisable?.Invoke(); }
    public void InvokeOnDestroy(){ childOnDestroy?.Invoke(); }
    public void InvokeUpdateGUI(){ childUpdate?.Invoke(); }

    protected void LoadUxml(string path)
    {
        if (string.IsNullOrEmpty(path))
            path = "Assets/Edixor/Scripts/UI/EdixorTab/default.uxml";
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        if (visualTree == null)
        {
            Debug.LogError($"UI файл не найден по пути: {path}");
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
    }

    protected void LoadUss(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        StyleSheet sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
        if (sheet == null)
        {
            Debug.LogError($"StyleSheet не найден по пути: {path}");
            return;
        }
        root.styleSheets.Add(sheet);
    }

    protected void LoadHotKey(string path, Action action)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Путь к горячей клавише пустой или null."); return;
        }
        KeyActionData data = AssetDatabase.LoadAssetAtPath<KeyActionData>(path);
        if (data == null)
        {
            Debug.LogError($"KeyActionData не найден по пути: {path}"); return;
        }
        var service = container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting);
        if (!service.TryAddHotKey(Title, data))
        {
            Debug.Log($"[Skip] Дубликат '{data.Name}' для '{Title}'."); return;
        }
        var logic = new CustomKeyAction(container, action);
        hotKeyController.AddKey(new KeyAction(data, logic));
        OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
    }

    protected void LoadHotKeys(string[] paths, Action[] actions)
    {
        if (paths == null || paths.Length == 0) { Debug.LogError("Path array is null or empty."); return; }
        var service = container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting);
        var newLoads = new List<KeyActionData>();
        for (int i = 0; i < paths.Length; i++)
        {
            var p = paths[i]; var a = actions != null && i < actions.Length ? actions[i] : null;
            if (string.IsNullOrEmpty(p)) continue;
            KeyActionData d = AssetDatabase.LoadAssetAtPath<KeyActionData>(p);
            if (d == null) continue;
            if (!service.TryAddHotKey(Title, d)) { Debug.Log($"[Skip] Дубликат '{d.Name}' для '{Title}'."); continue; }
            newLoads.Add(d);
            hotKeyController.AddKey(new KeyAction(d, new CustomKeyAction(container, a)));
        }
        if (newLoads.Count > 0)
        {
            OnHotKeyAdded?.Invoke(new HotKeyTabInfo(tabName));
            Debug.Log($"Добавлено {newLoads.Count} новых горячих клавиш для '{Title}'.");
        }
        else Debug.LogWarning("Новых горячих клавиш не было загружено.");
    }

    // Восстанавливаем методы, используемые SettingTab
    protected void ChangeStyle(int index)
    {
        container.ResolveNamed<StyleService>(ServiceNames.StyleSetting).SetCurrentItem(index);
    }

    protected void ChangeLayout(int index)
    {
        container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).SetCurrentItem(index);
    }

    public void DeleteUI()
    {
        root = null;
        InvokeOnDestroy();
    }
}
