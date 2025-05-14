using UnityEngine;
using UnityEngine.UIElements;

public static class ExStyleExtensions
{
    public static void ApplyWithStates(this ExStyle style, VisualElement element)
    {
        if (style == null || element == null) return;

        element.focusable = true;
        style.normal.ApplyTo(element);

        element.RegisterCallback<MouseEnterEvent>(evt => style.hover.ApplyTo(element));
        element.RegisterCallback<MouseLeaveEvent>(evt => style.normal.ApplyTo(element));
        element.RegisterCallback<MouseDownEvent>(evt => style.active.ApplyTo(element), TrickleDown.TrickleDown);
        element.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (element.worldBound.Contains(evt.mousePosition))
                style.hover.ApplyTo(element);
            else
                style.normal.ApplyTo(element);
        });
    }

    public static void ApplyScrollStyleWithStates(this ExScrollStyle style, VisualElement element)
    {
        if (style == null || element == null) return;

        style.normal.ApplyTo(element);

        element.RegisterCallback<MouseEnterEvent>(_ => style.hover.ApplyTo(element));
        element.RegisterCallback<MouseLeaveEvent>(_ => style.normal.ApplyTo(element));
        element.RegisterCallback<MouseDownEvent>(_ => style.active.ApplyTo(element), TrickleDown.TrickleDown);
        element.RegisterCallback<MouseUpEvent>(evt =>
        {
            if (element.worldBound.Contains(evt.mousePosition))
                style.hover.ApplyTo(element);
            else
                style.normal.ApplyTo(element);
        });
    }
}
