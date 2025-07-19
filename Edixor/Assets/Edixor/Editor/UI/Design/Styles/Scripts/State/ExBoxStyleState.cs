using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExBoxStyleState : IExStyleState
{
    public Color backgroundColor;

    public bool useSeparateBorderColors;
    public Color borderColor;
    public Color borderTopColor;
    public Color borderBottomColor;
    public Color borderLeftColor;
    public Color borderRightColor;

    public bool useSeparateBorderWidths;
    public float borderWidth;
    public float borderTopWidth;
    public float borderBottomWidth;
    public float borderLeftWidth;
    public float borderRightWidth;

    public float borderRadius;

    public void ApplyTo(VisualElement element)
    {
        element.style.backgroundColor = backgroundColor;

        if (useSeparateBorderColors)
        {
            element.style.borderTopColor = borderTopColor;
            element.style.borderBottomColor = borderBottomColor;
            element.style.borderLeftColor = borderLeftColor;
            element.style.borderRightColor = borderRightColor;
        }
        else
        {
            element.style.borderTopColor = borderColor;
            element.style.borderBottomColor = borderColor;
            element.style.borderLeftColor = borderColor;
            element.style.borderRightColor = borderColor;
        }

        if (useSeparateBorderWidths)
        {
            element.style.borderTopWidth = borderTopWidth;
            element.style.borderBottomWidth = borderBottomWidth;
            element.style.borderLeftWidth = borderLeftWidth;
            element.style.borderRightWidth = borderRightWidth;
        }
        else
        {
            element.style.borderTopWidth = borderWidth;
            element.style.borderBottomWidth = borderWidth;
            element.style.borderLeftWidth = borderWidth;
            element.style.borderRightWidth = borderWidth;
        }

        element.style.borderTopLeftRadius = borderRadius;
        element.style.borderTopRightRadius = borderRadius;
        element.style.borderBottomLeftRadius = borderRadius;
        element.style.borderBottomRightRadius = borderRadius;
    }
}
