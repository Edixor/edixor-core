using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;

public class RecommendedTabsUIFactory
{
    public VisualElement CreateRecommendedTabElement(Texture2D icon, string name, int usage, string path)
    {
        var root = new VisualElement();
        root.AddToClassList("recommended-tab-box");
        root.AddToClassList("E-box");

        var iconContainer = new VisualElement();
        iconContainer.AddToClassList("recommended-tab-icon");

        var iconImage = new Image();
        iconImage.AddToClassList("recommended-tab-image");
        iconImage.image = icon;

        iconContainer.Add(iconImage);
        root.Add(iconContainer);

        var content = new VisualElement();
        content.AddToClassList("recommended-tab-content");

        var topRow = new VisualElement();
        topRow.style.flexDirection = FlexDirection.Row;
        topRow.style.justifyContent = Justify.SpaceBetween;

        var nameLabel = new Label(name);
        nameLabel.AddToClassList("recommended-tab-name");

        var usageLabel = new Label("usage: " + usage);
        usageLabel.AddToClassList("recommended-tab-usage");
        usageLabel.AddToClassList("E-secondary-text");

        topRow.Add(nameLabel);
        topRow.Add(usageLabel);
        content.Add(topRow);

        var bottomRow = new VisualElement();
        bottomRow.style.flexDirection = FlexDirection.Row;
        bottomRow.style.justifyContent = Justify.SpaceBetween;
        bottomRow.style.alignItems = Align.Center; 
        bottomRow.style.marginTop = 4;

        var linkBox = new VisualElement();
        linkBox.AddToClassList("recommended-tab-linkbox");
        linkBox.AddToClassList("E-sub-box");

        var linkRow = new VisualElement();
        linkRow.style.flexDirection = FlexDirection.Row;
        linkRow.style.justifyContent = Justify.SpaceBetween;
        linkRow.style.alignItems = Align.Center;

        var pathLabel = new Label(path);
        pathLabel.AddToClassList("recommended-tab-path");
        pathLabel.AddToClassList("E-link");

        var copyIcon = new Image();
        copyIcon.AddToClassList("recommended-tab-copy");
        copyIcon.image = EditorGUIUtility.IconContent("TreeEditor.Duplicate").image;  
        copyIcon.RegisterCallback<ClickEvent>(_ =>
        {
            GUIUtility.systemCopyBuffer = path;
            Debug.Log($"Copied path: {path}");
        });

        linkRow.Add(pathLabel);
        linkRow.Add(copyIcon);
        linkBox.Add(linkRow);

         
        var openLabel = new Label("Open");
        openLabel.AddToClassList("recommended-tab-open");
        openLabel.RegisterCallback<ClickEvent>(_ =>
        {
            EditorApplication.ExecuteMenuItem(path);
        });

         
        bottomRow.Add(linkBox);
        bottomRow.Add(openLabel);
        content.Add(bottomRow);

        root.Add(content);
        return root;
    }

}
