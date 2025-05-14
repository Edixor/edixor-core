using UnityEngine.UIElements;
using UnityEngine;
using ExTools;
using System.Collections.Generic;

public class FactoryUIFunction
{
    private VisualElement _container;
    private readonly List<Button> _createdButtons = new List<Button>(); // Список для хранения созданных кнопок

    public void Init(VisualElement container)
    {
        _container = container;
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
        
        ExDebug.Log($"Creating button for function '{func.Data.Name}'.");
        _container.Add(button);
        _createdButtons.Add(button); // Сохраняем созданную кнопку в список
        ExDebug.Log($"container: {_container.name}");
        
        ExDebug.EndGroup();
    }

    public Button[] GetItems()
    {
        return _createdButtons.ToArray(); // Возвращаем массив созданных кнопок
    }
}