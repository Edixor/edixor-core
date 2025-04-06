using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorStyleSettings", menuName = "Edixor/StyleSettings", order = 1)]
public class StyleSaveAsset : ScriptableObject, ISaveCurrentAsset<StyleData>
{
    [SerializeField] private List<StyleData> styles = new();

    public List<StyleData> SaveItems
    {
        get => new List<StyleData>(styles);
        set => styles = value ?? new List<StyleData>();
    }

    [SerializeField] private int styleIndex = 0;

    public int CurrentIndex
    {
        get => styleIndex;
        set => styleIndex = value;
    }
}
