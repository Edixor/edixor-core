using UnityEngine;
using UnityEditor;

public static class ExUI
{
    public static void Background(Rect windowRect) 
    {
        Color backgroundColor = Color.black;
        EditorGUI.DrawRect(windowRect, backgroundColor);
    }
}
