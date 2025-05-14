using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

[Serializable]
public class NewTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/New Tab")]
    public static void ShowTab() => ShowTab<NewTab>();

    private void Awake()
    {
        Option("New Tab", "auto", "auto");
    }

    private void Start()
    {
        // Заполняем статичные элементы данными
        var versionLabel = root.Q<Label>("version-label");
        if (versionLabel != null)
            versionLabel.text = $"Version: {Application.version}";

        var updatesLabel = root.Q<Label>("updates-label");
        if (updatesLabel != null)
            updatesLabel.text = $"Recent updates: {DateTime.Now:dd.MM.yyyy}";
    }
}