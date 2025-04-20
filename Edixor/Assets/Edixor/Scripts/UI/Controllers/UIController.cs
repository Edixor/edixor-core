using UnityEngine.UIElements;
using UnityEngine;
public class UIController : IUIController
{
    private VisualElement _root;
    private VisualElement contentElement;
    private UIContent content;

    public UIController(VisualElement root = null)
    {
        _root = root;
    }

    public void InitRoot(VisualElement root)
    {
        if (root == null)
        {
            Debug.LogError("Root element is null. Cannot initialize UIController.");
            return;
        }

        _root = root;

        _root.Clear();
    }

    public bool IsRootInitialized()
    {
        return _root != null;
    }

    public void Show(UIContent content)
    {
        if (content == null)
        {
            Debug.LogError("Content is null. Cannot show UI.");
            return;
        }

        if (_root == null)
        {
            Debug.LogError("Root is not initialized. Call InitRoot() first.");
            return;
        }

        _root.Clear();

        contentElement = new VisualElement();
        content.Init(contentElement);

        VisualElement newContent = content.LoadUI();
        newContent.AddToClassList("content");
        newContent.name = "content";

        if (newContent == null)
        {
            Debug.LogError("Loaded UIContent returned null.");
            return;
        }

        contentElement.Add(newContent);
        contentElement.AddToClassList("content-container");
        contentElement.name = "content-container";
        _root.Add(contentElement);
    }


    public VisualElement GetElement(string name)
    {
        var element = contentElement.Q(name);
        if (element == null)
        {
            Debug.LogError($"Element with name {name} not found in UIController.");
        }
        return element;
    }
}