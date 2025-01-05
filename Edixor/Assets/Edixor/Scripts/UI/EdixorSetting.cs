using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "EdixorSettings", menuName = "Edixor/Settings", order = 1)]
public class EdixorSettings : ScriptableObject
{
    public int designIndex;
    public int designVersionIndex;
}