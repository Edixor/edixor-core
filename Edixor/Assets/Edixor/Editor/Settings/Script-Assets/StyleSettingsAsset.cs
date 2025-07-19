using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StyleSettings", menuName = "Edixor/Settings/StyleSettings", order = 1)]
public class StyleSettingsAsset : BaseSaveAsset<StyleData>, IDefaultSeedable
{
    [SerializeField]
    private int currentIndex = 0;

    public int CurrentIndex
    {
        get
        {
            if (Items == null || Items.Count == 0)
                return -1;
            return Mathf.Clamp(currentIndex, 0, Items.Count - 1);
        }
        set
        {
            if (Items != null && Items.Count > 0)
                currentIndex = Mathf.Clamp(value, 0, Items.Count - 1);
            else
                currentIndex = -1;
        }
    }

    public void EnsureDefaults()
    {
        if (Items != null && Items.Count > 0)
            return;

        var guids = AssetDatabase.FindAssets("t:StyleData");
        var allStyles = new List<StyleData>();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<StyleData>(path);
            if (data != null)
                allStyles.Add(data);
        }

        if (allStyles.Count > 0)
        {
            Items = allStyles;
            CurrentIndex = 0;
            Debug.Log($"[StyleSettingsAsset] Added {allStyles.Count} StyleData assets from the project.");
        }
        else
        {
            Debug.LogWarning("[StyleSettingsAsset] No StyleData assets found in the project.");
        }
    }
}
