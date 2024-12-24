using UnityEngine;
using UnityEditor;

public class SettingsFunction : EdixorFunction
{
    private EdixorUIManager edixorUIManager;

    public SettingsFunction(EdixorWindow window, EdixorUIManager edixorUIManager) : base(window)
    {
        this.edixorUIManager = edixorUIManager;
    }

    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_4_2024-12-24_18-59-17.jpg");

    public override string Name => "Setting";

    public override string Description => "Restarts the application or resets specific functionality.";

    public override void Activate()
    {
        Debug.Log("Setting functionality activated.");

        if (Window != null)
        {
            Window.GetUIManager().AddTab(new SettingTab(edixorUIManager.scrollView, edixorUIManager));
        }
    }

}
