using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[Serializable]
public class NewTab : EdixorTab
{

    public void Init(DIContainer container = null, VisualElement containerUI = null) {
        // Инициализация новой вкладки (при необходимости)
    }

    /// <summary>
    /// Специфичная логика отображения UI для вкладки NewTab.
    /// Вызывается базовым OnUI() после увеличения openCount.
    /// </summary>
    protected void OnTabUI() 
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
