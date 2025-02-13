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

    // Остальные поля, не нужные для показа в инспекторе:
    protected VisualElement ParentContainer;
    protected VisualElement root;

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

    public abstract void OnUI();

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

    protected T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object
    {
        T asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            Debug.LogError($"{typeof(T).Name} file not found at {path}.");
        }
        return asset;
    }

    public void SetParentContainer(VisualElement container)
    {
        ParentContainer = container;
    }
}
