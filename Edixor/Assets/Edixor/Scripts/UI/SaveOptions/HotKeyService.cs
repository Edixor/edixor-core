using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class HotKeyService : EdixorSetting<HotKeySaveAsset>
{
    public HotKeyService() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/HotKeySettings.asset")) { }

    public KeyActionData[] GetHotKeysByKey(string key)
    {
        EnsureDictionaryIsUpToDate();
        if (settings.HotKeyDictionary.TryGetValue(key, out var keyActions))
        {
            return keyActions;
        }

        Debug.LogWarning($"Ключ '{key}' не найден в словаре HotKeyDictionary.");
        return null;
    }

    public void AddHotKeyToDictionary(string key, KeyActionData[] keyActions)
    {
        EnsureDictionaryIsUpToDate();

        if (settings.Entries.Any(e => e.Key == key))
        {
            Debug.LogWarning($"Ключ '{key}' уже существует.");
            return;
        }

        settings.Entries.Add(new HotKeySaveAsset.KeyActionDictionaryEntry
        {
            Key = key,
            Values = keyActions
        });

        settings.RebuildDictionaryFromList();
        Save();
    }

    public void RemoveHotKeyFromDictionary(string key)
    {
        EnsureDictionaryIsUpToDate();

        var entry = settings.Entries.FirstOrDefault(e => e.Key == key);
        if (entry != null)
        {
            settings.Entries.Remove(entry);
            settings.RebuildDictionaryFromList();
            Save();
        }
        else
        {
            Debug.LogWarning($"Ключ '{key}' не найден.");
        }
    }

    public void UpdateHotKeyInDictionary(string key, KeyActionData[] newKeyActions)
    {
        EnsureDictionaryIsUpToDate();

        var entry = settings.Entries.FirstOrDefault(e => e.Key == key);
        if (entry != null)
        {
            entry.Values = newKeyActions;
            settings.RebuildDictionaryFromList();
            Save();
        }
        else
        {
            Debug.LogWarning($"Ключ '{key}' не найден.");
        }
    }

    public List<KeyActionData> GetAllHotKeys()
    {
        EnsureDictionaryIsUpToDate();
        return settings.HotKeyDictionary.Values.SelectMany(actions => actions).ToList();
    }

    private void EnsureDictionaryIsUpToDate()
    {
        if (settings.HotKeyDictionary == null || settings.HotKeyDictionary.Count != settings.Entries.Count)
        {
            settings.RebuildDictionaryFromList();
        }
    }

    public List<HotKeySaveAsset.KeyActionDictionaryEntry> GetAllEntries()
    {
        EnsureDictionaryIsUpToDate();
        return new List<HotKeySaveAsset.KeyActionDictionaryEntry>(settings.Entries);
    }
}
