using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class RestartFunction : EdixorFunction, IFunctionSetting
{
    private bool TabCleaning;
    private bool ClearCache;

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
        Toggle tabCleaningToggle = new Toggle("Tab cleaning");

        tabCleaningToggle.RegisterValueChangedCallback(evt =>
        {
            TabCleaning = evt.newValue; 
        });

        Toggle clearCacheToggle = new Toggle("Clear cache");
        
        clearCacheToggle.RegisterValueChangedCallback(evt =>
        {
            ClearCache = evt.newValue;
        });

        root.Add(tabCleaningToggle);
        root.Add(clearCacheToggle);
    }

}
