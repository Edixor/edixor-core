using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class CMItemMenu : BaseCMItem
{
    public BaseSelectionMenu menu { get; private set; }
    public bool IsSelected { get; private set; }

    public CMItemMenu(string name, bool isInteractive, BaseSelectionMenu menu, string key)
    {
        Name = name;
        IsInteractive = isInteractive;
        this.menu = menu;

        interactiveKey = $"{key}_IsInteractive";
        LoadState();
    }

    public void SetSelected(bool selected)
    {
        IsSelected = selected;
        SaveState();
    }

    public override VisualElement Draw()
    {
        root = CreateRootElement();
        return root;
    }
}