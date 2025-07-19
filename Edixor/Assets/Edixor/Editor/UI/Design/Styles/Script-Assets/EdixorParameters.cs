using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorParameters", menuName = "Edixor/Style/Parameters", order = 1)]
public class EdixorParameters : StyleParameters
{
    [SerializeField]
    BoxStyleEntry[] LayoutStyles = new BoxStyleEntry[]
    {
        new BoxStyleEntry("tab-section"),
        new BoxStyleEntry("middle-section-content"),
        new BoxStyleEntry("function-section"),
        new BoxStyleEntry("status-section")
    };
    [SerializeField]
    BoxStyleEntry[] TabContentBoxStyles = new BoxStyleEntry[]
    {
        new BoxStyleEntry("E-box"),
        new BoxStyleEntry("E-sub-box"),
        new BoxStyleEntry("E-box-background")
    };
    [SerializeField]
    LabelStyleEntry[] TabContentLabelStyles = new LabelStyleEntry[]
    {
        new LabelStyleEntry("E-label"),
        new LabelStyleEntry("E-link"),
        new LabelStyleEntry("E-error-text"),
        new LabelStyleEntry("E-warning-text"),
        new LabelStyleEntry("E-secondary-text")
    };
    [SerializeField]
    ButtonStyleEntry[] TabContentButtonStyles = new ButtonStyleEntry[]
    {
        new ButtonStyleEntry("E-button"),
        new ButtonStyleEntry("E-special-button")
    };
    [SerializeField]
    ExTabStyle[] TabStyles = new ExTabStyle[]
    {
        new ExTabStyle("normal"),
        new ExTabStyle("active"),
        new ExTabStyle("warning"),
        new ExTabStyle("error")
    };
    [SerializeField]
    ExFunctionStyle[] FunctionStyle = new ExFunctionStyle[]
    {
        new ExFunctionStyle("normal"),
        new ExFunctionStyle("warning"),
        new ExFunctionStyle("error")
    };
    [SerializeField]
    private ScrollStyleEntry[] scrollStyleEntries = new ScrollStyleEntry[]
    {
        new ScrollStyleEntry("E-Scroll")
    };
    [SerializeField] ButtonStyleEntry AddTabButtonStyle = new ButtonStyleEntry("AddTab");
    [SerializeField] ExScrollStyle SliderStyle;
    public IEnumerable<IStyleEntry> Styles
    {
        get
        {
            List<IStyleEntry> all = new List<IStyleEntry>();
            for (int i = 0; i < LayoutStyles.Length; i++) all.Add(LayoutStyles[i]);
            for (int i = 0; i < TabContentBoxStyles.Length; i++) all.Add(TabContentBoxStyles[i]);
            for (int i = 0; i < TabContentLabelStyles.Length; i++) all.Add(TabContentLabelStyles[i]);
            for (int i = 0; i < TabContentButtonStyles.Length; i++) all.Add(TabContentButtonStyles[i]);
            for (int i = 0; i < TabStyles.Length; i++) all.Add(TabStyles[i]);
            return all;
        }
    }

    public IEnumerable<BoxStyleEntry> Layout => LayoutStyles;
    public IEnumerable<BoxStyleEntry> ContentBoxes => TabContentBoxStyles;
    public IEnumerable<LabelStyleEntry> ContentLabels => TabContentLabelStyles;
    public IEnumerable<ButtonStyleEntry> ContentButtons => TabContentButtonStyles;
    public IEnumerable<ExTabStyle> Tabs => TabStyles;
    public IEnumerable<ExFunctionStyle> Functions => FunctionStyle;
    public ExScrollStyle Scroll => SliderStyle;
    public ButtonStyleEntry AddTabButton => AddTabButtonStyle;
    public IEnumerable<IScrollStyleEntry> ScrollStyles => scrollStyleEntries;

    public TEntry GetStyleByName<TEntry>(IEnumerable<TEntry> collection, string name) where TEntry : IStyleEntry
    {
        foreach (TEntry entry in collection)
        {
            if (entry.Name == name)
                return entry;
        }
        return default;
    }

}