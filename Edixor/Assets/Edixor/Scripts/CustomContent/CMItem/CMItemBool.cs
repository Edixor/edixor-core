using UnityEditor;
using UnityEngine;

public class CMItemBool : ICMItem
{
    public string Name { get; }
    public bool IsInteractive { get; set; }
    public bool IsSelected { get; private set; }

    private string selectedKey;
    private string interactiveKey;

    public CMItemBool(string name, bool isInteractive, bool value, string key)
    {
        Name = name;
        IsInteractive = isInteractive;
        IsSelected = value;

        interactiveKey = $"{key}_IsInteractive";
        selectedKey = $"{key}_IsSelected";
        LoadState();
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        SaveState();
    }

    public void SaveState()
    {
        EditorPrefs.SetBool(interactiveKey, IsInteractive);
        EditorPrefs.SetBool(selectedKey, IsSelected);
    }

    public void LoadState()
    {
        if (EditorPrefs.HasKey(interactiveKey))
        {
            IsInteractive = EditorPrefs.GetBool(interactiveKey);
        }

        if (EditorPrefs.HasKey(selectedKey))
        {
            IsSelected = EditorPrefs.GetBool(selectedKey);
        }
    }

    public void Draw<T, I>(Menu<T, I> menu, float itemHeight, GUIStyle style) where T : Menu<T, I> where I : ICMItem
    {
        string displayText = IsSelected ? $"{Name} - On" : $"{Name} - Off";

        if (IsInteractive)
        {
            if (GUILayout.Button(displayText, style, GUILayout.Height(itemHeight)))
            {
                menu.HandleItemSelection(this);
            }
        }
        else
        {
            GUILayout.Label(displayText, style, GUILayout.Height(itemHeight));
        }
    }
}
