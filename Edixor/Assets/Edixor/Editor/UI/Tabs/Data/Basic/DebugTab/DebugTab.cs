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
        Option("Debug", "auto", "auto", "Resources/Images/Icons/debug.png");
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
                var sep = new VisualElement();
                sep.AddToClassList("separator");
                parent.Add(sep);

                var groupBox = new Box();
                groupBox.AddToClassList("box");
                groupBox.style.marginLeft = indent * 10;

                var contentContainer = new VisualElement();
                contentContainer.AddToClassList("children-container");
                contentContainer.style.display = group.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;

                var headerRow = new VisualElement();
                headerRow.AddToClassList("row");

                var label = new Label($"[{group.Timestamp:HH:mm:ss}] [Group] {group.Header}");
                label.AddToClassList("group-label");
                headerRow.Add(label);
                groupBox.Add(headerRow);

                var arrow = new VisualElement();
                arrow.AddToClassList("toggle-icon");
                arrow.transform.rotation = Quaternion.identity;

                var icon = new Image();
                icon.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/arrow-top.png");
                icon.AddToClassList("toggle-icon");
                icon.style.scale = new Scale(new Vector3(1, 1, 1));

                var btn = new Button(() =>
                {
                    group.IsExpanded = !group.IsExpanded;
                    contentContainer.style.display = group.IsExpanded ? DisplayStyle.Flex : DisplayStyle.None;

                    icon.style.unityBackgroundImageTintColor = group.IsExpanded ? Color.white : Color.gray;
                    icon.style.scale = new Scale(new Vector3(1, group.IsExpanded ? -1 : 1, 1));
                })
                { name = "toggle-button" };

                btn.AddToClassList("E-button");
                btn.Add(icon);
                groupBox.Add(btn);
                groupBox.Add(contentContainer);
                parent.Add(groupBox);

                foreach (var child in group.Children)
                    BuildItem(child, indent + 1, contentContainer);
            }
            else if (item is DebugEntry entry)
            {
                var entryBox = new Box();
                entryBox.AddToClassList("box");
                entryBox.AddToClassList("entry-box");
                entryBox.style.marginLeft = indent * 10;

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


    private Texture2D FlipTextureVertically(Texture2D original)
    {
        if (original == null) return null;

        Texture2D flipped = new Texture2D(original.width, original.height);
        for (int y = 0; y < original.height; y++)
        {
            for (int x = 0; x < original.width; x++)
            {
                flipped.SetPixel(x, original.height - y - 1, original.GetPixel(x, y));
            }
        }
        flipped.Apply();
        return flipped;
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
