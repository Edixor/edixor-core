using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public class CloseFunction : EdixorFunction, IFunctionSetting
{
    public CloseFunction(EdixorWindow window) : base(window) {
        
    }
    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_3_2024-12-24_18-59-17.jpg");

    [Header("Basic information")]
    [SerializeField]
    private string _functionName = "Close";
    public string FunctionName
    {
        get { return _functionName; }
        private set { _functionName = value; }
    }

    public override string Name => _functionName;

    public override string Description => "Shows All Hotkeys in Aoplication";

    public override void Activate()
    {
        if (Window != null)
        {
            Window.MinimizeWindow();
        }
    }

    public void Setting(VisualElement root)
    {
        
    }
}
