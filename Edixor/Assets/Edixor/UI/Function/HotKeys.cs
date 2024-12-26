using UnityEngine;
using UnityEditor;

public class HotKeysFunction : EdixorFunction
{
    private EdixorUIManager edixorUIManager;

    public HotKeysFunction(EdixorWindow window, EdixorUIManager edixorUIManager) : base(window)
    {
        this.edixorUIManager = edixorUIManager;
    }

    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_4_2024-12-24_18-59-17.jpg");

    public override string Name => "HotKeys";

    public override string Description => "Shows All Hotkeys in Aoplication";

    public override void Activate()
    {
        

        if (Window != null)
        {
            Window.GetUIManager().AddTab(new HotKeysTab(edixorUIManager.scrollView, edixorUIManager));
        }
    }

}
