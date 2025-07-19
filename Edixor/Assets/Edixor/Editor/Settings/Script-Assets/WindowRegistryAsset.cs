using UnityEditor;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "WindowRegistry", menuName = "Edixor/Settings/WindowRegistry", order = 1)]
public class EdixorRegistryAsset : BaseSaveAsset<EdixorRegistryEntry>
{
    public void Register(EdixorRegistryEntry entry)
    {
        if (entry == null || string.IsNullOrEmpty(entry.Id)) return;
        var existing = Items.Find(e => e.Id == entry.Id);
        if (existing != null) Items.Remove(existing);
        Items.Add(entry);
        EditorUtility.SetDirty(this);
    }

    public void Unregister(string Id)
    {
        var existing = Items.Find(e => e.Id == Id);
        if (existing != null)
        {
            Items.Remove(existing);
            EditorUtility.SetDirty(this);
        }
    }
}
