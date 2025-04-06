using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StyleParameters", menuName = "Edixor/Style/Parameters", order = 1)]
public class StyleParameters : ScriptableObject
{
    [SerializeField] private Color[] colors;
    [SerializeField] private Color[] backgroundColors;
    [SerializeField] private Color[] borderColors;
    [SerializeField] private List<ElementStyleEntry> elementStylesList;
    [SerializeField] private Color functionIconColors;
    [SerializeField] private Color functionBackgroundColors;
    [SerializeField] private Font font;

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
                foreach (var entry in elementStylesList)
                {
                    elementStyles[entry.Key] = entry.Indices;
                }
            }
            return elementStyles;
        }
    }

    public Color FunctionIconColors => functionIconColors;
    public Color FunctionBackgroundColors => functionBackgroundColors;

    [System.Serializable]
    public class ElementStyleEntry
    {
        public string Key;
        public int[] Indices = new int[2]; 
    }
}

