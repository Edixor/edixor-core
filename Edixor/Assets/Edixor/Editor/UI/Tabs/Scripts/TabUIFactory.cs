using UnityEngine.UIElements;
using UnityEngine;
using ExTools;

public class TabUIFactory
{
    public VisualElement CreateTabContainer(EdixorTab tab, EdixorParameters parameters, System.Action onSwitch, System.Action onClose)
    {
        ExDebug.BeginGroup("Create Function: " + tab.GetType().Name);

        if (tab == null)
        {
            ExDebug.LogError("Tab is null. Cannot create tab container.");
            return null;
        }
        var containerElement = new VisualElement();
        containerElement.AddToClassList("tab-container");

        ExDebug.Log($"Creating tab container for '{tab.GetType().Name}'.");

        string title = string.IsNullOrEmpty(tab.Title) ? tab.GetType().Name : tab.Title;

        var tabButton = new Button(onSwitch)
        {
            text = title
        };
        tabButton.AddToClassList("tab-button");
        tabButton.name = "tab-button";

        ExDebug.Log($"Creating tab button with title '{title}'.");

        var closeButton = new Button(onClose)
        {
            text = "X"
        };
        closeButton.AddToClassList("tab-button-exit");
        closeButton.name = "tab-button-exit";

        ExDebug.Log($"Creating close button for tab '{title}'.");

        parameters.TabStyle.ApplyWithStates(tabButton);
        parameters.TabStyle.ApplyWithStates(closeButton);

        containerElement.Add(tabButton);
        containerElement.Add(closeButton);

        ExDebug.EndGroup();
        return containerElement;
    }
}
