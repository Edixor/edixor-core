using UnityEditor;
using UnityEngine;
using System;

public class CMItemAction : ICMItem
{
    public string Name { get; private set; }
    public bool IsInteractive { get; set; }
    public Action Delegate { get; private set; }

    private string interactiveKey;

    public CMItemAction(string name, bool isInteractive, Action action, string key)
    {
        Name = name;
        IsInteractive = isInteractive;
        Delegate = action;

        interactiveKey = $"{key}_IsInteractive";
        LoadState();
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
                Delegate.Invoke();
                menu.CloseMenu();
            }
        }
        else
        {
            GUILayout.Label(Name, style, GUILayout.Height(itemHeight));
        }
    }
}
