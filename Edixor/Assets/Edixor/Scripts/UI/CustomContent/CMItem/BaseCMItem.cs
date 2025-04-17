using UnityEditor;
using UnityEngine;

public abstract class BaseCMItem : ICMItem
{
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

    public abstract void Draw();
}