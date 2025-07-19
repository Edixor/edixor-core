using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExFunctionStyleState : IExStyleState
{
    public Color iconColor;
    public Color backgroundColor;
    public Color borderColor;
    public float borderWidth;
    public float borderRadius;

    public void ApplyTo(VisualElement element)
    {
        element.style.unityBackgroundImageTintColor = iconColor;
        element.style.backgroundColor = backgroundColor;
        element.style.borderTopColor = element.style.borderBottomColor =
            element.style.borderLeftColor = element.style.borderRightColor = borderColor;
        element.style.borderTopWidth = element.style.borderBottomWidth =
            element.style.borderLeftWidth = element.style.borderRightWidth = borderWidth;
        element.style.borderTopLeftRadius = element.style.borderTopRightRadius =
            element.style.borderBottomLeftRadius = element.style.borderBottomRightRadius = borderRadius;
    }
}
