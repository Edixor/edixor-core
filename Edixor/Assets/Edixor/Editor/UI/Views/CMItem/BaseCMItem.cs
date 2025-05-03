using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public abstract class BaseCMItem : ICMItem
{
    protected VisualElement root;
    public string Name { get; protected set; }
    public bool IsInteractive { get; set; }
    public string interactiveKey { get; protected set; }

    public virtual void SaveState()
    {
        EditorPrefs.SetBool(interactiveKey, IsInteractive);
    }

    public virtual void LoadState()
    {
        if (EditorPrefs.HasKey(interactiveKey))
        {
            IsInteractive = EditorPrefs.GetBool(interactiveKey);
        }
    }

    protected VisualElement CreateRootElement()
    {
        VisualElement container = new VisualElement();
        container.AddToClassList("item-root");
        container.name = "item-root";
        return container;
    }

    public abstract VisualElement Draw();
}