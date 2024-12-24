using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class RestartFunction : EdixorFunction, IFunctionSetting
{
    public RestartFunction(EdixorWindow window) : base(window)
    {
    }

    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_2_2024-12-24_18-59-17.jpg");

    public override string Name => "Restart";

    public override string Description => "Restarts the application or resets specific functionality.";

    public bool deleteWindow;

    public override void Activate()
    {
        if (Window != null)
        {
            Window.RestartWindow(); 
        }
    }

    public void Setting(VisualElement root)
    {
        VisualElement boxSetting = new VisualElement();
        
        boxSetting.Add(new Label(Name));
        boxSetting.Add(new Toggle("Tab cleaning"));
        boxSetting.Add(new Toggle("Clear cache"));

        root.Add(boxSetting);
    }
}
