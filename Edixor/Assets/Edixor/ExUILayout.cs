using UnityEngine;
using UnityEditor;

public static class ExUILayout
{
    public static void Background(Rect windowRect) 
    {
        Color backgroundColor = Color.black;
        EditorGUI.DrawRect(windowRect, backgroundColor);
    }
}
