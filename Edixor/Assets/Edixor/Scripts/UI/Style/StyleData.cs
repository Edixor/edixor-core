using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EdixorStyle", menuName = "Edixor/Style/Data", order = 0)]
public class StyleData : ScriptableObject
{
    [SerializeField] private string styleName;
    [TextArea] [SerializeField] private string pathParameter;
    [SerializeField] private StyleParameters assetParameter;

    public string Name => styleName;

    public StyleParameters GetAssetParameter()
    {
        if (assetParameter == null)
        {
            assetParameter = AssetDatabase.LoadAssetAtPath<StyleParameters>(PathResolver.ResolvePath(pathParameter));
            if (assetParameter == null)
            {
                Debug.LogError($"Style parameter not found at path: {pathParameter}");
            }
        }
        else
        {
            Debug.Log($"Style parameter already loaded: {assetParameter}");
            if (string.IsNullOrWhiteSpace(pathParameter))
            {
                pathParameter = AssetDatabase.GetAssetPath(assetParameter);
            }
        }
        return assetParameter;
    }

    public void SetParameter(StyleParameters newParameter)
    {
        if (newParameter == null)
        {
            Debug.Log("Style parameter is null. Cannot set it.");
            return;
        }
        assetParameter = newParameter;
    }
}
