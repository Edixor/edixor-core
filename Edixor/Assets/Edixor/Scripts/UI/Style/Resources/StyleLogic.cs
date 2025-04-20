using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StyleLogic
{
    private StyleParameters _parameters;
    private VisualElement _root;

    public StyleLogic(VisualElement root = null, StyleParameters parameter = null)
    {
        this._root = root;
        this._parameters = parameter;
    }

    public void Init(VisualElement root = null, StyleParameters parameter = null)
    {
        if (root != null)
        {
            this._root = root;
        }

        if (parameter != null)
        {
            this._parameters = parameter;
        }

        if (this._root == null || this._parameters == null)
        {
            Debug.LogError("Root or parameters are null.");
            return;
        }

        foreach (var styleRule in _parameters.ElementStyles)
        {
            string className = styleRule.Key;
            int textColorIndex = styleRule.Value[0];
            int backgroundColorIndex = styleRule.Value[1];

            var styledElements = _root.Query<VisualElement>(className).ToList();

            if (styledElements == null || styledElements.Count == 0)
            {
                Debug.LogWarning($"No elements found with class '{className}' for styling.");
                continue;
            }

            foreach (var element in styledElements)
            {
                if (element == null)
                {
                    Debug.LogWarning($"Null element found for class '{className}'.");
                    continue;
                }

                element.style.color = _parameters.Colors[textColorIndex];
                element.style.backgroundColor = _parameters.BackgroundColors[backgroundColorIndex];
            }
        }
    }

    public void FunctionStyling(List<Button> list)
    {
        /*if (list == null || _parameters == null)
        {
            Debug.LogError("List or parameters are null.");
            return;
        }

        foreach (var button in list)
        {
            if (button == null)
            {
                Debug.LogWarning("Null button encountered in FunctionStyling.");
                continue;
            }

            Debug.Log($"Styling button: {button.name}");

            button.style.backgroundColor = _parameters.FunctionBackgroundColors;
            button.style.unityBackgroundImageTintColor = _parameters.FunctionIconColors;
        }
        */
    }
}
