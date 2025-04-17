using UnityEditor;
using UnityEngine;

public class CMItemBool : BaseCMItem
{
    public bool IsSelected { get; private set; }
    private string selectedKey;

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

    public override void SaveState()
    {
        EditorPrefs.SetBool(interactiveKey, IsInteractive);
        EditorPrefs.SetBool(selectedKey, IsSelected);
    }

    public override void LoadState()
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

    public override void Draw()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(Name, GUILayout.Width(150));
        bool newValue = GUILayout.Toggle(IsSelected, "");
        if (newValue != IsSelected)
        {
            SetSelected(newValue);
        }
        GUILayout.EndHorizontal();
    }
}