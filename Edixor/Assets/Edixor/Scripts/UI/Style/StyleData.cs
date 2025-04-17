using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorStyle", menuName = "Edixor/Style/Data", order = 0)]
public class StyleData : ScriptableObject
{
    [SerializeField] private string styleName;
    [TextArea] [SerializeField] private string pathParameter;
    [SerializeField] private List<StyleParameters> assetParameters = new List<StyleParameters>();

    public string Name => styleName;

    public List<StyleParameters> GetAssetParameters()
    {
        if (assetParameters == null || assetParameters.Count == 0)
        {
            assetParameters = LoadParametersFromPath(pathParameter);
            if (assetParameters == null || assetParameters.Count == 0)
            {
                Debug.LogError($"No style parameters found at path: {pathParameter}");
            }
        }
        else
        {
            Debug.Log($"Style parameters already loaded: {assetParameters.Count} items.");
            if (string.IsNullOrWhiteSpace(pathParameter))
            {
                pathParameter = AssetDatabase.GetAssetPath(assetParameters[0]);
            }
        }
        return assetParameters;
    }

    public void SetParameters(List<StyleParameters> newParameters)
    {
        if (newParameters == null || newParameters.Count == 0)
        {
            Debug.Log("Style parameters list is null or empty. Cannot set it.");
            return;
        }
        assetParameters = newParameters;
    }

    private List<StyleParameters> LoadParametersFromPath(string path)
    {
        List<StyleParameters> parameters = new List<StyleParameters>();
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.LogError("Path is empty or null. Cannot load style parameters.");
            return parameters;
        }

        string[] guids = AssetDatabase.FindAssets("t:StyleParameters", new[] { path });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            StyleParameters parameter = AssetDatabase.LoadAssetAtPath<StyleParameters>(assetPath);
            if (parameter != null)
            {
                parameters.Add(parameter);
            }
        }

        return parameters;
    }
}
