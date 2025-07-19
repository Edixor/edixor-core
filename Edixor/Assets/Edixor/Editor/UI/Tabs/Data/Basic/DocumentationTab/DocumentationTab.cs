using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using ExTools;
using System;

[Serializable]
public class DocumentationTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Documentation")]
    public static void ShowTab() => ShowTab<DocumentationTab>();

    private ScrollView _docsList;
    private TextField _searchField;

    private List<string> _allDocs = new(); 

    public void Awake()
    {
        Option("Documentation", "auto", "auto", "Resources/Images/Icons/docs.png");
    }

    public void Start()
    {
        _searchField = root.Q<TextField>("search-docs");
        _docsList = root.Q<ScrollView>("docs-list");

        if (_searchField == null || _docsList == null)
        {
            Debug.LogError("DocumentationTab: UI elements not found in UXML");
            return;
        }

        _searchField.RegisterValueChangedCallback(evt => RenderDocs());

        RenderDocs();  
    }

    private void RenderDocs()
    {
        _docsList.Clear();

        var filter = _searchField.value?.ToLowerInvariant() ?? string.Empty;

        int count = 0;
        foreach (var doc in _allDocs)
        {
            if (!doc.ToLowerInvariant().Contains(filter)) continue;

            var label = new Label(doc);
            label.AddToClassList("extension-desc");
            _docsList.Add(label);
            count++;
        }

        if (count == 0)
        {
            var emptyLabel = new Label("No documentations found. (x_x)");
            emptyLabel.AddToClassList("empty-label");
            _docsList.Add(emptyLabel);
        }
    }
}
