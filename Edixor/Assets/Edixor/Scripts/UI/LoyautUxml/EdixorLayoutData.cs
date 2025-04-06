using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EdixorLayout", menuName = "Edixor/Layout", order = 3)]
public class EdixorLayoutData : ScriptableObject
{
    [SerializeField]
    private string layoutName;
    [SerializeField]
    private string layoutDescription;
    [SerializeField]
    private string pathUss;
    [SerializeField]
    private string pathUxml;

    public string Name => layoutName;
    public string Description => layoutDescription;
    public string PathUss => pathUss;
    public string PathUxml => pathUxml;
    
    [TextArea] 
    public string Logica;


}