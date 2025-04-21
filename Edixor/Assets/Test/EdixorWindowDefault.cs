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
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Exit.asset");
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Minimize.asset");
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Restart.asset");
    }
}

