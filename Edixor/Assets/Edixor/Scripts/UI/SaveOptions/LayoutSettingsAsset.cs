using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorLayoutDataSettings", menuName = "Edixor/LayoutSettings", order = 1)]
public class LayoutSaveAsset : ScriptableObject
{
    [SerializeField]
    private List<LayoutData> layouts = new();

    public List<LayoutData> SaveItems
    {
        get => layouts;
        set => layouts = value ?? new List<LayoutData>();
    }

    [SerializeField]
    private int layoutIndex = 0;

    public int CurrentIndex
    {
        get => layoutIndex;
        set => layoutIndex = Mathf.Clamp(value, 0, layouts.Count - 1);
    }
}
