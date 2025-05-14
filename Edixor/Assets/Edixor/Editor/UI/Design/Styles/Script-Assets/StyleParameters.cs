using UnityEngine;
using System.Collections.Generic;

public class StyleParameters : ScriptableObject
{
    [SerializeField] protected Color[] colors;
    [SerializeField] protected Color[] backgroundColors;
    [SerializeField] protected Color[] borderColors;
    [SerializeField] protected List<ElementStyleEntry> styleEntry; 
    [SerializeField] protected Font[] font;

    private Dictionary<string, int[]> elementStyles;

    public Color[] Colors => colors;
    public Color[] BackgroundColors => backgroundColors;
    public Color[] BorderColors => borderColors;

    public Dictionary<string, int[]> ElementStyles
    {
        get
        {
            if (elementStyles == null)
            {
                elementStyles = new Dictionary<string, int[]>();

                foreach (ElementStyleEntry entry in styleEntry)
                {
                    elementStyles[entry.Key] = entry.Indices;
                }
            }
            return elementStyles;
        }
    }

    [System.Serializable]
    public class ElementStyleEntry
    {
        public string Key;
        public int[] Indices = new int[2];
    }
}

