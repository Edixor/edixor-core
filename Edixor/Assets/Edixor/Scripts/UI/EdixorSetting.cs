using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorSettings", menuName = "Edixor/Settings", order = 1)]
public class EdixorSettings : ScriptableObject
{
    [SerializeReference] public List<EdixorDesign> designs;
    public int designIndex;
    public int designVersionIndex;
    
    [SerializeReference] public List<EdixorFunction> functions;

    [SerializeReference] public List<KeyAction> hotKeys;

    public bool isModified;

    public bool isWindowOpen;
}
 