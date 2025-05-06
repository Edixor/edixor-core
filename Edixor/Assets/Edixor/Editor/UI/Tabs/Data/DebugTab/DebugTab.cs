using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using ExTools;
using System.Linq;
using System.Collections.Generic;

public class DebugTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Debug")]
    public static void ShowTab() => ShowTab<DebugTab>();

    private ScrollView _logScroll;
    private TextField _detailField;

    private void Awake()
    {
        Option("Debug", "auto", "auto");
    }

    private void Start()
    {
        _logScroll   = root.Q<ScrollView>("log-scroll");
        _detailField = root.Q<TextField>("detail-field");

        RefreshList();
    }

    private void RefreshList()
    {
        _logScroll.Clear();
        var history = ExDebug.GetHistory().ToList();

        void BuildItem(DebugItem item, int indent, VisualElement parent)
        {
            if (item is DebugGroup group)
            {
                // Separator above group
                var sep = new VisualElement();
                sep.AddToClassList("separator");
                parent.Add(sep);

                // Group box container
                var groupBox = new Box();
                groupBox.AddToClassList("box");
                groupBox.style.marginLeft = indent * 10;
                groupBox.style.paddingTop = 4;
                groupBox.style.paddingBottom = 4;
                groupBox.style.position = Position.Relative;

                // Content container declared early for closure
                var contentContainer = new VisualElement();
                contentContainer.AddToClassList("children-container");
                contentContainer.style.display = group.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                contentContainer.style.flexDirection = FlexDirection.Column;
                contentContainer.style.marginTop = 4;

                // Header row
                var headerRow = new VisualElement();
                headerRow.AddToClassList("row");
                headerRow.style.flexDirection = FlexDirection.Row;
                headerRow.style.alignItems = Align.Center;

                // Timestamp + group label
                var label = new Label($"[{group.Timestamp:HH:mm:ss}] [Group] {group.Header}");
                label.AddToClassList("group-label");
                label.style.flexGrow = 1;
                headerRow.Add(label);

                groupBox.Add(headerRow);

                // Toggle button (square icon placeholder)
                var arrow = new VisualElement();
                arrow.AddToClassList("toggle-icon");
                arrow.style.width = 12;
                arrow.style.height = 12;
                arrow.transform.rotation = group.IsExpanded ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;

                var btn = new Button(() =>
                {
                    group.IsExpanded = !group.IsExpanded;
                    contentContainer.style.display = group.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;
                    arrow.transform.rotation = group.IsExpanded ? Quaternion.Euler(0, 0, 90) : Quaternion.identity;
                })
                { name = "toggle-button" };
                btn.Add(arrow);
                btn.style.position = Position.Absolute;
                btn.style.right = 4;
                btn.style.top = 4;
                groupBox.Add(btn);

                // Add the content container
                groupBox.Add(contentContainer);
                parent.Add(groupBox);

                // Recursively build children
                foreach (var child in group.Children)
                    BuildItem(child, indent + 1, contentContainer);
            }
            else if (item is DebugEntry entry)
            {
                var entryBox = new Box();
                entryBox.AddToClassList("box");
                entryBox.style.marginLeft = indent * 10;
                entryBox.style.paddingTop = 2;
                entryBox.style.paddingBottom = 2;

                var txt = new Label($"[{entry.Timestamp:HH:mm:ss}] {entry.Header}");
                txt.AddToClassList(entry.Severity.ToString().ToLower());
                txt.AddToClassList("entry-label");

                int idx = history.IndexOf(item);
                txt.userData = idx;
                txt.RegisterCallback<MouseUpEvent>(evt => ShowDetails((int)txt.userData));

                entryBox.Add(txt);
                parent.Add(entryBox);
            }
        }

        foreach (var item in history)
        {
            if (!history.OfType<DebugGroup>().Any(g => g.Children.Contains(item)))
                BuildItem(item, 0, _logScroll);
        }

        if (history.Count > 0)
            ShowDetails(0);
    }

    private void ShowDetails(int index)
    {
        var history = ExDebug.GetHistory();
        var item = history[index];

        if (item is DebugEntry e)
        {
            _detailField.value =
                $"Time:      {e.Timestamp:yyyy-MM-dd HH:mm:ss}\n" +
                $"Severity:  {e.Severity}\n" +
                $"Caller:    {e.Caller} (line {e.LineNumber})\n\n" +
                $"StackTrace:\n{e.StackTrace}";
        }
        else if (item is DebugGroup g)
        {
            _detailField.value = $"Group: {g.Header}\nChildren: {g.Children.Count}";
        }
    }
}
