using UnityEditor;
using UnityEngine;
using System;

public class CMItemAction : BaseCMItem
{
    public Action Delegate { get; private set; }

    public CMItemAction(string name, bool isInteractive, Action action, string key)
    {
        Name = name;
        IsInteractive = isInteractive;
        Delegate = action;

        interactiveKey = $"{key}_IsInteractive";
        LoadState();
    }

    public override void Draw()
    {
        if (IsInteractive)
        {
            if (GUILayout.Button(Name))
            {
                Delegate?.Invoke();
                EditorWindow.focusedWindow?.Close();
            }
        }
        else
        {
            GUILayout.Label(Name, EditorStyles.boldLabel);
        }
    }
}