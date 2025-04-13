using UnityEngine.UIElements;

public class TabFactory
{
    public VisualElement CreateTabContainer(EdixorTab tab, System.Action onSwitch, System.Action onClose)
    {
        VisualElement containerElement = new VisualElement();
        containerElement.AddToClassList("tab-container");

        string title = string.IsNullOrEmpty(tab.Title) ? tab.GetType().Name : tab.Title;

        var tabButton = new Button(() => onSwitch())
        {
            text = title
        };
        tabButton.AddToClassList("tab-button");
        containerElement.Add(tabButton);

        var closeButton = new Button(() => onClose())
        {
            text = "X"
        };
        closeButton.AddToClassList("tab-button-exit");
        containerElement.Add(closeButton);

        return containerElement;
    }
}
