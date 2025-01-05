using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
public class SettingsFunction : EdixorFunction
{
    public SettingsFunction(EdixorWindow window) : base(window)
    {
    }

    public override Texture2D Icon => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Edixor/Texture/EdixorWindow/Functions/photo_4_2024-12-24_18-59-17.jpg");

    public override string Name => "Setting";

    public override string Description => "Restarts the application or resets specific functionality.";

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

            SettingTab settingTab = new SettingTab(ParentContainer, Window);
            Window.GetUIManager().AddTab(settingTab);
        }
    }
}
