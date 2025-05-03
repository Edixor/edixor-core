using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorStyle", menuName = "Edixor/Style/Data", order = 0)]
public class StyleData : ScriptableObject
{
    [SerializeField] private string styleName;
    [SerializeField] private StyleParameters[] assetParameters = new StyleParameters[0];

    public string Name => styleName;
    public StyleParameters[] AssetParameters => assetParameters;

}