using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[Serializable]
public class NewTab : EdixorTab
{
    public NewTab(VisualElement ParentContainer)
        : base(ParentContainer, "New Tab", 
               "Assets/Edixor/Scripts/UI/EdixorTab/NewTab/NewTab.uxml", 
               "Assets/Edixor/Scripts/UI/EdixorTab/NewTab/NewTab.uss")
    {}

    public override void OnUI() 
    {
        VisualElement designContainer = root.Q<VisualElement>("basic-data");
        VisualElement tabContainer = root.Q<VisualElement>("frequent-tabs");
        VisualElement newsContainer = root.Q<VisualElement>("news");
        if (designContainer == null)
        {
            Debug.LogError("Design container not found.");
            return;
        }

        VisualElement informationBox = new VisualElement();
        informationBox.AddToClassList("...");
        informationBox.Add(new Label("version: " + "0.1"));
        informationBox.Add(new Label("recent updates: " + "15.10.2025"));
        Button documentation = new Button() { text = "Documentation" };
        informationBox.Add(documentation);

        designContainer.Add(informationBox);
    }
}
