using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorParameters", menuName = "Edixor/Style/Edixor Parameters", order = 1)]
public class EdixorParameters : StyleParameters
{
    [SerializeField] private ExStyle tabStyle;
    [SerializeField] private ExStyle addTabStyle;
    [SerializeField] private ExStyle functionStyle;
    [SerializeField] private ExScrollStyle scrollStyle;

    [SerializeField] private List<StyleEntry> styles = new List<StyleEntry>();
    [SerializeField] private List<StyleScrollEntry> scrollStyles = new List<StyleScrollEntry>();
    public List<StyleEntry> Styles => styles;
    public List<StyleScrollEntry> ScrollStyles => scrollStyles;

    public ExStyle TabStyle => tabStyle;
    public ExStyle AddTabStyle => addTabStyle;
    public ExStyle FunctionStyle => functionStyle;
    public ExScrollStyle ScrollStyle => scrollStyle;

    public ExStyle GetStyle(string name)
    {
        foreach (var entry in styles)
        {
            if (entry.Name == name)
                return entry.Style;
        }
        return null; 
    }

    public ExScrollStyle GetScrollStyle(string name)
    {
        foreach (var entry in scrollStyles)
        {
            if (entry.Name == name)
                return entry.Style;
        }
        return null; 
    }

    [System.Serializable]
    public class StyleEntry
    {
        public string Name;
        public ExStyle Style;
    }

    [System.Serializable]
    public class StyleScrollEntry
    {
        public string Name;
        public ExScrollStyle Style;
    }
}
