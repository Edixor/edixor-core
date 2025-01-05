using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public abstract class EdixorTab
{
    public abstract string Title { get; }
    public abstract string PathUxml { get; }
    public abstract string PathUss { get; }
    protected VisualElement ParentContainer;

    protected EdixorTab(VisualElement ParentContainer)
    {
        this.ParentContainer = ParentContainer;
    }

    public virtual void LoadUI() 
    {
        var visualTree = LoadAssetAtPath<VisualTreeAsset>(PathUxml);
        if (visualTree == null)
        {
            Debug.LogError("UI file not found.");
            return;
        }

        VisualElement root = visualTree.Instantiate();
        root.style.height = new StyleLength(Length.Percent(100));

        if (ParentContainer == null)
        {
            Debug.LogError("'ParentContainer' is null. Cannot load NewTab UI.");
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

    public virtual void DeleteUI() {
        if (ParentContainer == null)
        {
            Debug.LogError("'ParentContainer' is null. Cannot delete NewTab UI.");
            return;
        }

        ParentContainer.Clear();
        Debug.Log("NewTab UI has been successfully removed from the parent container.");
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

}
