using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorSettings", menuName = "Edixor/Settings", order = 1)]
public class EdixorSettingSave : ScriptableObject
{
    [SerializeReference] public List<EdixorDesign> designs;
    public int designIndex;
    public int designVersionIndex;
    
    [SerializeReference] public List<EdixorFunction> functions;

    [SerializeReference] public List<KeyAction> hotKeys;

    // Новое поле для хранения открытых вкладок
    [SerializeReference] public List<EdixorTab> tabs = new List<EdixorTab>();

    // Новое поле для хранения индекса активной вкладки
    public Rect originalWindowRect;
    public int lastActiveTabIndex;

    public bool isModified;

    public bool isWindowOpen;
}
