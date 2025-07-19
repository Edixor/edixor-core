using UnityEngine.UIElements;
using UnityEngine;
using ExTools;
using System.Collections.Generic;

public class FunctionUIFactory
{
    private VisualElement _container;
    private readonly List<Button> _createdButtons = new List<Button>();
    private ExFunctionStyle _functionStyle;

    public void Init(VisualElement container, ExFunctionStyle functionStyle)
    {
        _container = container;
        _functionStyle = functionStyle;
    }

    public void Clear()
    {
        if (_container == null) return;

        foreach (var button in _createdButtons)
        {
            _container.Remove(button);
        }
        _createdButtons.Clear();
    }

    public void Create(Function func)
    {
        ExDebug.BeginGroup("Create Function: " + func.Data.Name);
        if (_container == null) return;

        var button = new Button(() => func.Logic.Activate());
        button.AddToClassList("function");

        if (func.Data.Icon is Texture2D tex)
            button.style.backgroundImage = new StyleBackground(tex);
        else
            ExDebug.LogWarning("Icon is not a Texture2D.");

        if (_functionStyle != null)
            _functionStyle.ApplyWithStates(button);

        _container.Add(button);
        _createdButtons.Add(button);
        ExDebug.Log($"container: {_container.name}");
        ExDebug.EndGroup();
    }

    public Button[] GetItems()
    {
        return _createdButtons.ToArray();
    }
}
