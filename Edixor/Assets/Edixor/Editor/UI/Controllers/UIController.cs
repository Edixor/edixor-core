using UnityEngine.UIElements;
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
        Style = container.ResolveNamed<StyleSetting>(ServiceNames.StyleSetting).GetCorrectItem();
        Layout = container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting).GetCorrectItem();
    }

    public void InitRoot(VisualElement root)
    {
        if (root == null)
        {
            ExDebug.LogError("Root element is null. Cannot initialize UIController.");
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
            ExDebug.LogError("Content is null. Cannot show UI.");
            return;
        }

        if (_root == null)
        {
            ExDebug.LogError("Root is not initialized. Call InitRoot() first.");
            return;
        }

        _root.Clear();

        contentElement = new VisualElement();
        content.Init(contentElement, Layout, Style);

        VisualElement newContent = content.LoadUI();
        newContent.AddToClassList("content");
        newContent.name = "content";

        if (newContent == null)
        {
            ExDebug.LogError("Loaded UIContent returned null.");
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
            ExDebug.LogError($"Element with name {name} not found in UIController.");
        }
        return element;
    }
}