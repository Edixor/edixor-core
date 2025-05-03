using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using ExTools;
using System;

[Serializable]
public class NewTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/New Tab")]
    public static void ShowTab()
    {
        ShowTab<NewTab>();
    }


    private void Awake()
    {
        tabName = "New Tab";
        LoadUxml("auto");
        LoadUss("auto");
    }

    private void Start() 
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
        informationBox.Add(new Label("recent updates " + "15.10.2025"));
        Button documentation = new Button() { text = "Documentation" };
        informationBox.Add(documentation);

        designContainer.Add(informationBox);
    }
}
