using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EdixorWindowDefault : EdixorWindow
{
    private const string WindowName = "EdixorDefault";

    [MenuItem("Window/Edixor Window/" + WindowName)]
    [MenuItem("Edixor/Window/" + WindowName)]
    public static void ShowExample()
    {
        ShowWindow<EdixorWindowDefault>(WindowName);
    }

    protected override void OnOptions()
    {
        setting.LoadHotKey("Exit", WindowName);
        setting.LoadHotKey("Minimize", WindowName);
        setting.LoadHotKey("Restart", WindowName);

        setting.LoadFunction("Close");
        setting.LoadFunction("HotKey");
        setting.LoadFunction("Setting");
        setting.LoadFunction("Restart");

        setting.SetLayout("Standard");
        setting.SetStyle("Unity");
    }
}

