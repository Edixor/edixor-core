using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public class HotKeysFunction : EdixorFunction, IFunctionSetting
{
    private EdixorUIManager edixorUIManager;
    private List<KeyAction> hotkeys;

    public HotKeysFunction(EdixorWindow window) : base(window) {
        
    }
    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_3_2024-12-24_18-59-17.jpg");

    [Header("Basic information")]
    [SerializeField]
    private string _functionName = "HotKeys";
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
        hotkeys = Window.GetSetting().GetHotKeys();

        foreach (KeyAction key in hotkeys)
        {
            string combination = string.Join(" + ", key.Combination); 

            Label hotkeyLabel = new Label($"{key.Name}: {combination}");
            Button hotkeyEdit = new Button() { text = "Edit" };

            root.Add(hotkeyLabel);
            root.Add(hotkeyEdit);
        }
    }
}
