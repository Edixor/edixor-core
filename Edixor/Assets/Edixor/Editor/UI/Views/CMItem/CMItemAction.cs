using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

public class CMItemAction : BaseCMItem
{
    public Action Delegate { get; private set; }

    public CMItemAction(string name, bool isInteractive, Action action)
    {
        Name = name;
        IsInteractive = isInteractive;
        Delegate = action;

        interactiveKey = $"{name}_IsInteractive";
        LoadState();
    }

    public override VisualElement Draw()
    {
        root = CreateRootElement();

        Button button = new Button(() =>
        {
            if (IsInteractive)
            {
                Delegate?.Invoke();
            }
        })
        {
            text = Name
        };

        button.AddToClassList("item-button");
        button.name = "item-button";
        button.style.height = 20f;
        button.style.borderTopLeftRadius = 0;
        button.style.borderTopRightRadius = 0;
        button.style.borderBottomLeftRadius = 0;
        button.style.borderBottomRightRadius = 0;
        root.Add(button);

        return root;
    }
}