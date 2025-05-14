using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExStyleState
{
    public Color textColor;
    public Color backgroundColor;
    public Color borderTopColor;
    public Color borderBottomColor;
    public Color borderLeftColor;
    public Color borderRightColor;
    public float borderWidth;
    public float borderRadius;
    public Font font;


    public void ApplyTo(VisualElement element)
    {
        element.style.color             = textColor;
        element.style.backgroundColor   = backgroundColor;
        element.style.borderTopWidth    = borderWidth;
        element.style.borderBottomWidth = borderWidth;
        element.style.borderLeftWidth   = borderWidth;
        element.style.borderRightWidth  = borderWidth;
        element.style.borderTopColor    = borderTopColor;
        element.style.borderBottomColor = borderBottomColor;
        element.style.borderLeftColor   = borderLeftColor;
        element.style.borderRightColor  = borderRightColor;
        element.style.borderTopLeftRadius     = borderRadius;
        element.style.borderTopRightRadius    = borderRadius;
        element.style.borderBottomLeftRadius  = borderRadius;
        element.style.borderBottomRightRadius = borderRadius;
        element.style.unityFont = font;
        element.style.unityFontDefinition = font != null ? FontDefinition.FromFont(font) : new FontDefinition();
    }
}