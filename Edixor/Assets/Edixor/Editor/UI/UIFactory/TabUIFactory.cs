using UnityEngine.UIElements;
using System;
using ExTools.Controllers;

using ExTools;

public class TabUIFactory
{
    public VisualElement CreateTabContainer(EdixorTab logic, ExTabStyle styleEntry, Action onSwitch, Action onClose)
    {
        var container = new VisualElement();
        container.AddToClassList("tab-container");

        var btn = new Button(onSwitch) { name = "tab-button" };
        btn.AddToClassList("tab-button");

        var wrapper = new VisualElement();
        wrapper.style.flexDirection = FlexDirection.Row;
        wrapper.style.alignItems = Align.Center;

        var icon = new Image { name = "tab-icon", image = logic.Icon };
        icon.AddToClassList("tab-icon");
        wrapper.Add(icon);

        var label = new Label(logic.Title) { name = "tab-title" };
        label.AddToClassList("tab-title");
        wrapper.Add(label);

        var closeBtn = new Button(onClose) { name = "tab-button-exit", text = "X" };
        closeBtn.AddToClassList("tab-button-exit");
        closeBtn.RegisterCallback<ClickEvent>(evt => evt.StopPropagation(), TrickleDown.TrickleDown);
        wrapper.Add(closeBtn);

        btn.Add(wrapper);

        container.Add(btn);
        styleEntry.ApplyWithStates(container);

        return container;
    }
}
