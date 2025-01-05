using UnityEngine.UIElements;
using UnityEngine;

[System.Serializable]
public class ExUITElements
{
    public TypeElements typeElements;
    public StyleSheet displayOptions;

    public StyleSheet GetGUIStyle() {
        return displayOptions;
    }
}
