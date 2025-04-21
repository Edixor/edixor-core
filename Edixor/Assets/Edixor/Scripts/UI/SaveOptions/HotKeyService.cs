using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class HotKeySetting : EdixorSetting<HotKeySaveAsset>
{
    public HotKeySetting() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/HotKeySettings.asset")) { }
    private void EnsureDictionaryIsUpToDate()
    {
        if (Settings.HotKeyDictionary == null || Settings.HotKeyDictionary.Count != Settings.Entries.Count)
        {
            Settings.RebuildDictionaryFromList();
        }
    }

    public bool TryAddHotKey(string key, KeyActionData data)
    {
        return TryAddHotKeys(key, new[] { data });
    }

    public bool TryAddHotKeys(string key, KeyActionData[] newDatas)
    {
        EnsureDictionaryIsUpToDate();

        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key);
        var toAdd = new List<KeyActionData>();

        if (entry != null)
        {
            foreach (var data in newDatas)
            {
                bool exists = entry.Values.Any(x =>
                    x.Name == data.Name &&
                    x.Combination.SequenceEqual(data.Combination)
                );
                if (!exists)
                    toAdd.Add(data);
            }

            if (toAdd.Count > 0)
            {
                var list = entry.Values.ToList();
                list.AddRange(toAdd);
                entry.Values = list.ToArray();
            }
        }
        else
        {
            Settings.Entries.Add(new HotKeySaveAsset.KeyActionDictionaryEntry
            {
                Key = key,
                Values = newDatas
            });
            toAdd.AddRange(newDatas);
        }

        if (toAdd.Count > 0)
        {
            Settings.RebuildDictionaryFromList();
            SaveSettings();
            return true;
        }

        return false;
    }

    public List<KeyActionData> GetAllHotKeys()
    {
        EnsureDictionaryIsUpToDate();
        return Settings.HotKeyDictionary.Values.SelectMany(arr => arr).ToList();
    }

    public KeyActionData[] GetHotKeysByKey(string key)
    {
        EnsureDictionaryIsUpToDate();
        if (Settings.HotKeyDictionary.TryGetValue(key, out var keyActions))
            return keyActions;
        return null;
    }

    public List<HotKeySaveAsset.KeyActionDictionaryEntry> GetAllEntries()
    {
        EnsureDictionaryIsUpToDate();
        return new List<HotKeySaveAsset.KeyActionDictionaryEntry>(Settings.Entries);
    }

    public void RemoveHotKeyFromDictionary(string key)
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

    public void UpdateHotKeyInDictionary(string key, KeyActionData[] newKeyActions)
    {
        EnsureDictionaryIsUpToDate();
        var entry = Settings.Entries.FirstOrDefault(e => e.Key == key);
        if (entry != null)
        {
            entry.Values = newKeyActions;
            Settings.RebuildDictionaryFromList();
            SaveSettings();
        }
    }
}