using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StyleSettings", menuName = "Edixor/Settings/StyleSettings", order = 1)]
public class StyleSettingsAsset : BaseSaveAsset<StyleData>
{
    [SerializeField]
    private int currentIndex;

    public int CurrentIndex
    {
        get => Mathf.Clamp(currentIndex, 0, Items.Count - 1);
        set => currentIndex = Mathf.Clamp(value, 0, Items.Count - 1);
    }
}
