using HotKeySettingsBase = EdixorSettingsData<HotKeySaveAsset, KeyActionData, HotKeyId>;
using SettingEntries = ISettingEntries<HotKeySaveAsset.KeyActionDictionaryEntry, string>;
using SettingItemFull = ISettingItemFull<KeyAction, HotKeyId>;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class HotKeySetting : HotKeySettingsBase, SettingEntries, SettingItemFull
{
    public HotKeySetting() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/HotKeySettings.asset")) { }

    private void EnsureDictionaryIsUpToDate()
    {
        if (Settings.HotKeyDictionary == null || Settings.HotKeyDictionary.Count != Settings.Entries.Count)
        {
            Settings.RebuildDictionaryFromList();
        }
    }

    public override void AddItem(KeyActionData item, HotKeyId key)
    {

        EnsureDictionaryIsUpToDate();

        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key.title);
        if (entry == null)
        {
            Settings.Entries.Add(new HotKeySaveAsset.KeyActionDictionaryEntry {
                Key = key.title,
                Values = new[]{ item }
            });
        }
        else
        {
            bool already = entry.Values
                .Any(v => v.Name.Equals(key.name, StringComparison.OrdinalIgnoreCase));
            if (already) 
                return;

            var list = entry.Values.ToList();
            list.Add(item);
            entry.Values = list.ToArray();
        }

        Settings.RebuildDictionaryFromList();
        SaveSettings();
    }


    public override void RemoveItem(HotKeyId key)
    {
        EnsureDictionaryIsUpToDate();

        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key.title);
        if (entry != null)
        {
            var values = entry.Values.ToList();
            var removed = values.RemoveAll(v => v.Name == key.name);

            if (removed > 0)
            {
                if (values.Count == 0)
                    Settings.Entries.Remove(entry);
                else
                    entry.Values = values.ToArray();

                Settings.RebuildDictionaryFromList();
                SaveSettings();
            }
        }
    }

    public override void UpdateItem(HotKeyId key, KeyActionData item)
    {
        EnsureDictionaryIsUpToDate();

        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key.title);
        if (entry != null)
        {
            var values = entry.Values.ToList();
            var index = values.FindIndex(v => v.Name == key.name);
            if (index >= 0)
            {
                values[index] = item;
                entry.Values = values.ToArray();

                Settings.RebuildDictionaryFromList();
                SaveSettings();
            }
        }
    }

    public override KeyActionData GetItem(HotKeyId key)
    {
        EnsureDictionaryIsUpToDate();

        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key.title);
        if (entry != null)
        {
            return entry.Values.FirstOrDefault(v => v.Name == key.name);
        }
        return null;
    }

    public override KeyActionData[] GetAllItem()
    {
        EnsureDictionaryIsUpToDate();

        var allItems = new List<KeyActionData>();
        foreach (var entry in Settings.Entries)
        {
            allItems.AddRange(entry.Values);
        }
        return allItems.ToArray();
    }

    public KeyAction GetItemFull(HotKeyId key)
    {
        EnsureDictionaryIsUpToDate();

        return null;
    }

    public KeyAction[] GetAllItemFull()
    {
        EnsureDictionaryIsUpToDate();

        return null;
    }

    public HotKeySaveAsset.KeyActionDictionaryEntry GetEntries(string key)
    {
        EnsureDictionaryIsUpToDate();

        return Settings.Entries.FirstOrDefault(e => e.Key == key);
    }
    
    public HotKeySaveAsset.KeyActionDictionaryEntry GetEntry(string key)
    {
        EnsureDictionaryIsUpToDate();
        return Settings.Entries.FirstOrDefault(e => e.Key == key);
    }

    public IReadOnlyList<HotKeySaveAsset.KeyActionDictionaryEntry> GetAllEntries()
    {
        EnsureDictionaryIsUpToDate();
        return Settings.Entries.ToList();
    }

    public void RemoveEntry(string key)
    {
        EnsureDictionaryIsUpToDate();
        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key);
        if (entry != null)
        {
            Settings.Entries.Remove(entry);
            Settings.RebuildDictionaryFromList();
            SaveSettings();
        }
    }

    public void UpdateEntry(string key, HotKeySaveAsset.KeyActionDictionaryEntry updated)
    {
        EnsureDictionaryIsUpToDate();
        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key);
        if (entry != null)
        {
            entry.Values = updated.Values;
            Settings.RebuildDictionaryFromList();
            SaveSettings();
        }
    }
}