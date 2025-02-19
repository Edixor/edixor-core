using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using System;

[Serializable]
public abstract class EdixorTab
{
    // Эти поля будут отображаться в инспекторе в заданном порядке:
    [SerializeField, Tooltip("Название вкладки")]
    private string tabName;

    [SerializeField, Tooltip("Путь к UXML файлу")]
    private string pathUxml;

    [SerializeField, Tooltip("Путь к USS файлу")]
    private string pathUss;

    // Счётчик, показывающий, сколько раз вкладка была открыта
    [SerializeField, Tooltip("Сколько раз вкладка была открыта пользователем")]
    private int openCount = 0;

    // Флаг, показывающий, что вкладка уже активирована (чтобы не увеличивать счетчик повторно)
    [NonSerialized]
    private bool _isActive = false;

    // Остальные поля, не нужные для показа в инспекторе:
    protected VisualElement ParentContainer;
    protected VisualElement root;
    protected EdixorWindow window;

    protected EdixorTab(VisualElement ParentContainer, string tabName, string pathUxml, string pathUss)
    {
        this.ParentContainer = ParentContainer;
        this.tabName = tabName;
        this.pathUxml = pathUxml;
        this.pathUss = pathUss;
    }

    /// <summary>
    /// Название вкладки (только для чтения)
    /// </summary>
    public string Title => tabName;

    /// <summary>
    /// Путь к UXML файлу
    /// </summary>
    public string PathUxml => pathUxml;

    /// <summary>
    /// Путь к USS файлу
    /// </summary>
    public string PathUss => pathUss;

    /// <summary>
    /// Количество открытий вкладки.
    /// Это значение может сохраняться вместе с настройками.
    /// </summary>
    public int OpenCount => openCount;

    /// <summary>
    /// Вызывается один раз при создании вкладки.
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// Вызывается при каждом открытии вкладки.
    /// Если вкладка еще не была активирована в данном цикле, увеличивает счетчик и помечает вкладку как активную.
    /// После этого вызывается метод OnTabUI(), содержащий специфичную логику вкладки.
    /// </summary>
    public virtual void OnUI() 
    {
        if (!_isActive)
        {
            openCount++;
            _isActive = true;
            Debug.Log($"Вкладка '{tabName}' открыта {openCount} раз(а).");
        }
        OnTabUI();
    }

    /// <summary>
    /// Абстрактный метод для реализации логики отображения UI вкладки.
    /// Должен быть реализован в наследниках.
    /// </summary>
    protected abstract void OnTabUI();

    /// <summary>
    /// Сбрасывает состояние активности вкладки.
    /// Вызывается при переключении вкладок, чтобы при повторном открытии вкладки счетчик увеличивался снова.
    /// </summary>
    public void Deactivate()
    {
        _isActive = false;
    }

    /// <summary>
    /// Загружает UI вкладки из UXML и USS файлов.
    /// </summary>
    public virtual void LoadUI() 
    {
        var visualTree = LoadAssetAtPath<VisualTreeAsset>(PathUxml);
        if (visualTree == null)
        {
            Debug.LogError("UI file not found.");
            return;
        }

        root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("'ParentContainer' is null. Cannot load Tab UI.");
            return;
        }

        ParentContainer.Clear();
        ParentContainer.Add(root);

        var styleSheet = LoadAssetAtPath<StyleSheet>(PathUss);
        if (styleSheet == null)
        {
            Debug.LogError("StyleSheet file not found.");
            return;
        }
        root.styleSheets.Add(styleSheet);
    }

    /// <summary>
    /// Удаляет UI вкладки из родительского контейнера.
    /// </summary>
    public virtual void DeleteUI() 
    {
        if (ParentContainer == null)
        {
            Debug.LogError("'ParentContainer' is null. Cannot delete Tab UI.");
            return;
        }

        ParentContainer.Clear();
        Debug.Log("Tab UI has been successfully removed from the parent container.");
    }

    /// <summary>
    /// Вспомогательный метод для загрузки ассета по указанному пути.
    /// </summary>
    protected T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            Debug.LogError($"{typeof(T).Name} file not found at {path}.");
        }
        return asset;
    }

    /// <summary>
    /// Устанавливает родительский контейнер для UI вкладки.
    /// </summary>
    public void SetParentContainer(VisualElement container)
    {
        ParentContainer = container;
    }

    /// <summary>
    /// Устанавливает окно, в котором отображается вкладка.
    /// </summary>
    public void SetWindow(EdixorWindow window) 
    {
        this.window = window;
    }
}
