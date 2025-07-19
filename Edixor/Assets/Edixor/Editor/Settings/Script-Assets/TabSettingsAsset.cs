using UnityEngine;

[CreateAssetMenu(fileName = "TabSettings", menuName = "Edixor/Settings/Tab Settings", order = 4)]
public class TabSettingsAsset : BaseSaveAsset<TabData>
{
    [SerializeField]
    private int currentIndex;

    public int CurrentIndex
    {
        get => currentIndex;
        set => currentIndex = Mathf.Clamp(value, 0, Items.Count - 1);
    }
}
