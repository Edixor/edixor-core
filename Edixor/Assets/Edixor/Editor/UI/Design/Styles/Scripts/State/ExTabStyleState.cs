using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExTabStyleState : IExStyleState
{
    public ExBoxStyleState boxStyle;
    public ExLabelStyleState labelStyle;
    public ExButtonStyleState tabButtonStyle;
    public ExButtonStyleState closeButtonStyle;
    public Color iconColor;

    public void ApplyTo(VisualElement element)
    {
        boxStyle.ApplyTo(element);

        var tabButton = element.Q<Button>("tab-button");
        if (tabButton != null)
            tabButtonStyle.ApplyTo(tabButton);

        var label = element.Q<Label>("tab-title");
        if (label != null)
            labelStyle.ApplyTo(label);

        var closeBtn = element.Q<Button>("tab-button-exit");
        if (closeBtn != null)
            closeButtonStyle.ApplyTo(closeBtn);

        var icon = element.Q<Image>("tab-icon");
        if (icon != null)
            icon.tintColor = iconColor;
    }
}
