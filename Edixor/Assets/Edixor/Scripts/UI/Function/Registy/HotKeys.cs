using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class HotKeysFunction : EdixorFunction, IFunctionSetting
{
    private EdixorUIManager edixorUIManager;

    public HotKeysFunction(EdixorWindow window) : base(window) {
        
    }
    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_3_2024-12-24_18-59-17.jpg");

    public override string Name => "HotKeys";

    public override string Description => "Shows All Hotkeys in Aoplication";

    public override void Activate()
    {
        if (Window != null)
        {
            VisualElement ParentContainer = Window.GetUIManager().GetDesign().GetSection("middle-section-content");
            if (ParentContainer == null)
            {
                Debug.LogError("ParentContainer is null.");
                return;
            }

            HotKeysTab hotKeysTab = new HotKeysTab(ParentContainer, Window);
            Window.GetUIManager().AddTab(hotKeysTab);
        }
    }

    public void Setting(VisualElement root)
    {
       //...
    }
}
