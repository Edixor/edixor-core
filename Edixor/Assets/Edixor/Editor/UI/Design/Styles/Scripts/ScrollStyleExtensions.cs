using UnityEngine.UIElements;

namespace ExTools
{
    public static class ScrollStyleExtensions
    {
        public static void ApplyScrollStyleWithStates(this ExScrollStyle style, VisualElement element)
        {
            if (style == null || element == null) return;

            style.normal.ApplyTo(element);

            element.RegisterCallback<MouseEnterEvent>(_ => style.hover.ApplyTo(element));
            element.RegisterCallback<MouseLeaveEvent>(_ => style.normal.ApplyTo(element));

            element.RegisterCallback<MouseDownEvent>(_ => style.active.ApplyTo(element));
            element.RegisterCallback<MouseUpEvent>(_ => style.hover.ApplyTo(element));

            if (!element.enabledSelf)
                style.disabled.ApplyTo(element);

            element.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                if (element.enabledSelf)
                    style.normal.ApplyTo(element);
            });
        }
    }
}
