using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct ExLabelStyleState : IExStyleState
{
    public Color textColor;
    public Font font;
    public void ApplyTo(VisualElement element)
    {
        element.style.color = textColor;
        if(font!=null) element.style.unityFontDefinition = FontDefinition.FromFont(font);
    }
}