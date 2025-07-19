using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EdixorLayout", menuName = "Edixor/Layout/Data", order = 3)]
public class LayoutData : ScriptableObject
{
    [SerializeField]
    private string layoutName;
    [SerializeField]
    private string layoutDescription;
    [SerializeField]
    private StyleSheet uss;
    [SerializeField]
    private VisualTreeAsset uxml;
    [SerializeField]
    private LayoutParameters assetParameters;

    public string Name => layoutName;
    public string Description => layoutDescription;
    public StyleSheet LayoutStyleSheet => uss;
    public VisualTreeAsset LayoutVisualTreeAsset => uxml;
    public LayoutParameters AssetParameters => assetParameters;
}