using UnityEditor;
using UnityEngine;
using System;

public class CMItemMenu : ICMItem
{
    public string Name { get; }
    public bool IsInteractive { get; set; }
    public CustomMenu Menu { get; private set; }
    public bool IsSelected { get; private set; } // Added property for selection state

    private string interactiveKey;

    public CMItemMenu(string name, bool isInteractive, CustomMenu menu, string key)
    {
        Name = name;
        IsInteractive = isInteractive;
        Menu = menu;

        interactiveKey = $"{key}_IsInteractive";
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
    }

    public void LoadState()
    {
        if (EditorPrefs.HasKey(interactiveKey))
        {
            IsInteractive = EditorPrefs.GetBool(interactiveKey);
        }
    }

    public void Draw<T, I>(Menu<T, I> menu, float itemHeight, GUIStyle style) where T : Menu<T, I> where I : ICMItem
    {
        if (IsInteractive)
        {
            if (GUILayout.Button(Name, style, GUILayout.Height(itemHeight)))
            {
                menu.CloseMenu();
            }
        }
        else
        {
            GUILayout.Label(Name, style, GUILayout.Height(itemHeight));
        }
    }
}
