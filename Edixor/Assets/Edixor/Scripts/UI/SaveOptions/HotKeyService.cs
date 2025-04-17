using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class HotKeyService : EdixorSetting<HotKeySaveAsset>
{
    public HotKeyService()
        : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/HotKeySettings.asset"))
    { }

    private void EnsureDictionaryIsUpToDate()
    {
        if (settings.HotKeyDictionary == null || settings.HotKeyDictionary.Count != settings.Entries.Count)
        {
            settings.RebuildDictionaryFromList();
        }
    }

    public override void Save()
    {
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Пытается добавить одиночную горячую клавишу.
    /// Возвращает true, если комбинация была добавлена.
    /// </summary>
    public bool TryAddHotKey(string key, KeyActionData data)
    {
        return TryAddHotKeys(key, new[] { data });
    }

    /// <summary>
    /// Пытается добавить несколько комбинаций.
    /// Возвращает true, если добавлено хотя бы одно новое KeyActionData.
    /// </summary>
    public bool TryAddHotKeys(string key, KeyActionData[] newDatas)
    {
        EnsureDictionaryIsUpToDate();

        var entry = settings.Entries.FirstOrDefault(e => e.Key == key);
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
            settings.Entries.Add(new HotKeySaveAsset.KeyActionDictionaryEntry
            {
                Key = key,
                Values = newDatas
            });
            toAdd.AddRange(newDatas);
        }

        if (toAdd.Count > 0)
        {
            settings.RebuildDictionaryFromList();
            Save();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Возвращает все KeyActionData из словаря.
    /// </summary>
    public List<KeyActionData> GetAllHotKeys()
    {
        EnsureDictionaryIsUpToDate();
        return settings.HotKeyDictionary.Values.SelectMany(arr => arr).ToList();
    }

    public KeyActionData[] GetHotKeysByKey(string key)
    {
        EnsureDictionaryIsUpToDate();
        if (settings.HotKeyDictionary.TryGetValue(key, out var keyActions))
            return keyActions;
        return null;
    }

    public List<HotKeySaveAsset.KeyActionDictionaryEntry> GetAllEntries()
    {
        EnsureDictionaryIsUpToDate();
        return new List<HotKeySaveAsset.KeyActionDictionaryEntry>(settings.Entries);
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
    }
}