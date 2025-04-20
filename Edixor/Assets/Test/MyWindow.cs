using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EdixorWindowDefault : EdixorWindow
{
    [MenuItem("Window/Edixor Window/Default")]
    public static void ShowExample()
    {
        GetWindow<EdixorWindowDefault>("MyWindow");
    }

    protected override void Options()
    {
        
    }
}

