using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EdixorLayoutDataSettings", menuName = "Edixor/LayoutSettings", order = 1)]
public class LayoutSaveAsset : ScriptableObject, ISaveCurrentAsset<EdixorLayoutData>
{
    [SerializeField]
    private List<EdixorLayoutData> layouts = new();

    public List<EdixorLayoutData> SaveItems
    {
        get => layouts.ToList();
        set => layouts = value?.ToList() ?? new();
    }

    [SerializeField]
    private int layoutIndex = 0;

    public int CurrentIndex
    {
        get => layoutIndex;
        set => layoutIndex = Mathf.Clamp(value, 0, layouts.Count - 1);
    }
}
