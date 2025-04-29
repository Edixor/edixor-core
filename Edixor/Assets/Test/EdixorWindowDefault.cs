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
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Exit.asset", "Window");
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Minimize.asset", "Window");
        setting.LoadHotKey("Assets/Edixor/Scripts/UI/HotKeys/Registy/Resources/Standart/Restart.asset", "Window");

        setting.LoadFunction("Assets/Edixor/Scripts/UI/Function/Registy/Resources/Close.asset");
        setting.LoadFunction("Assets/Edixor/Scripts/UI/Function/Registy/Resources/HotKey.asset");
        setting.LoadFunction("Assets/Edixor/Scripts/UI/Function/Registy/Resources/Setting.asset");
        setting.LoadFunction("Assets/Edixor/Scripts/UI/Function/Registy/Resources/Restart.asset");
    }
}

