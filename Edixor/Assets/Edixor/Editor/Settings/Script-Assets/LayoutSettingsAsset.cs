using UnityEngine;

[CreateAssetMenu(fileName = "LayoutSettings", menuName = "Edixor/Settings/Layout Settings", order = 3)]
public class LayoutSettingsAsset : BaseSaveAsset<LayoutData>
{
    [SerializeField]
    private int currentIndex;

    public int CurrentIndex
    {
        get => Mathf.Clamp(currentIndex, 0, Items.Count - 1);
        set => currentIndex = Mathf.Clamp(value, 0, Items.Count - 1);
    }
}
