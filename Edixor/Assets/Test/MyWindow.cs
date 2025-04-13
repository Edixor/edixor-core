using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MyWindow : EdixorWindow
{
    [MenuItem("Window/Edixor Window/My Window")]
    public static void ShowExample()
    {
        GetWindow<MyWindow>("MyWindow");
    }

    public void CreateGUI()
    {
        
    }
}

