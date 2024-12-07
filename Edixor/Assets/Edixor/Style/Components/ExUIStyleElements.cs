using UnityEngine;

[System.Serializable]
public class BFPStyleElements
{
    public string name;
    public GUIStyle displayOptions;

    public GUIStyle GetGUIStyle() {
        return displayOptions;
    }
}
