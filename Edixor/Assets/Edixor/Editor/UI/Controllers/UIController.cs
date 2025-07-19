using UnityEngine.UIElements;
using UnityEngine;
using ExTools;

public class UIController : IUIController
{
    private VisualElement _root;
    private VisualElement contentElement;
    private UIContent content;
    private readonly DIContainer _container;
    public StyleData Style { get; private set; }
    public LayoutData Layout { get; private set; }

    public UIController(DIContainer container, VisualElement root = null)
    {
        _root = root;
        _container = container;
    }

    public void InitRoot(VisualElement root)
    {
        ExDebug.BeginGroup("UIController: Initialized");
        if (root == null)
        {
            ExDebug.LogError("Root element is null. Cannot initialize UIController.");
            return;
        }

        _root = root;

        _root.Clear();
    }

    public void SetStyle(StyleData style)
    {
        if (style == null)
        {
            ExDebug.LogError("Style is null. Cannot set style.");
            return;
        }

        Style = style;
    }

    public void SetLayout(LayoutData layout)
    {
        if (layout == null)
        {
            ExDebug.LogError("Layout is null. Cannot set layout.");
            return;
        }

        Layout = layout;
    }

    public bool IsRootInitialized()
    {
        return _root != null;
    }

    public void Show(UIContent content)
    {
        if (content == null)
        {
            ExDebug.LogError("Content is null. Cannot show UI.");
            return;
        }

        if (_root == null)
        {
            ExDebug.LogError("Root is not initialized. Call InitRoot() first.");
            return;
        }

        _root.Clear();

        Style = _container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting).GetCorrectItem();
        if (Style == null)
        {
            ExDebug.LogWarning("Style is null. Default styles will not be applied.");
        }
        else
        {
            ExDebug.Log("Style successfully resolved and applied.");
        }

        Layout = _container.ResolveNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting).GetCorrectItem();
        if (Layout == null)
        {
            ExDebug.LogWarning("Layout is null. Default layout will not be applied.");
        }
        else
        {
            ExDebug.Log("Layout successfully resolved and applied.");
        }

        contentElement = new VisualElement();
        content.Init(contentElement, Layout, Style, _container);

        ExDebug.Log("Loading UIContent: " + content.GetType().Name);
        VisualElement newContent = content.LoadUI();
        newContent.AddToClassList("content");
        newContent.name = "content";

        if (newContent == null)
        {
            ExDebug.LogError("Loaded UIContent returned null.");
            return;
        }
        else
        {
            ExDebug.Log("UIContent successfully loaded and added to the content container.");
        }

        contentElement.Add(newContent);
        contentElement.AddToClassList("content-container");
        contentElement.name = "content-container";
        _root.Add(contentElement);

        ExDebug.EndGroup();
    }

    public VisualElement GetElement(string name)
    {
        var element = contentElement.Q(name);
        if (element == null)
        {
            ExDebug.LogError($"Element with name {name} not found in UIController.");
        }
        return element;
    }
}