using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "LayoutSettings", menuName = "Edixor/Settings/Layout Settings", order = 3)]
public class LayoutSettingsAsset : BaseSaveAsset<LayoutData>, IDefaultSeedable
{
    [SerializeField] private int currentIndex = 0;
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

        var guids = AssetDatabase.FindAssets("t:LayoutData");
        var allLayouts = new List<LayoutData>();

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<LayoutData>(path);
            if (data != null)
                allLayouts.Add(data);
        }

        if (allLayouts.Count > 0)
        {
            Items = allLayouts;
            CurrentIndex = 0;
            Debug.Log($"[LayoutSettingsAsset] Found and added {allLayouts.Count} LayoutData assets from the project.");
        }
        else
        {
            Debug.LogWarning("[LayoutSettingsAsset] No LayoutData assets found in the project.");
        }
    }
}
