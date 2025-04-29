using UnityEngine;
using UnityEngine.UIElements;

public class FactoryUIFunction
{
    private VisualElement _container;

    public void Init(VisualElement container)
    {
        if (_container != null) return;
        _container = container;
    }

    public void Create(Function func, string contName)
    {
        VisualElement container = _container.Q<VisualElement>(contName);
        if (container == null) return;

        Button button = new Button(() => func.Logic.Activate());
        button.AddToClassList("function");

        Texture2D texture = func.Data.Icon as Texture2D;
        if (texture != null)
        {
            button.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogWarning("FactoryUIFunction: не удалось загрузить изображение по пути Resources/Path/To/Image");
        }

        container.Add(button);
    }
}
