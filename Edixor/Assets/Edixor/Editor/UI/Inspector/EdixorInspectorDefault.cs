using UnityEditor;

[CustomEditor(typeof(DemonstrationEdixorInspectorDefault))]
public class EdixorInspectorDefault : EdixorInspector
{
    protected override void OnOptions()
    {
        setting.LoadHotKey("Exit", "Inspector");
        setting.LoadHotKey("Restart", "Inspector");

        setting.LoadFunction("HotKey");
        setting.LoadFunction("Setting");
        setting.LoadFunction("Restart");

        setting.SetLayout("Standard");
        setting.SetStyle("Unity");
    }
}
