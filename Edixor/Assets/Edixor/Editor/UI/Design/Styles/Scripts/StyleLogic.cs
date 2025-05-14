using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using ExTools;

public class StyleLogic
{
    protected StyleParameters _parameters;
    protected VisualElement _root;

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
            ExDebug.LogError("Root or parameters are null.");
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
                ExDebug.LogWarning($"No elements found with class '{className}' for styling.");
                continue;
            }

            foreach (var element in styledElements)
            {
                if (element == null)
                {
                    ExDebug.LogWarning($"Null element found for class '{className}'.");
                    continue;
                }

                element.style.color = _parameters.Colors[textColorIndex];
                element.style.backgroundColor = _parameters.BackgroundColors[backgroundColorIndex];
            }
        }
    }
}
