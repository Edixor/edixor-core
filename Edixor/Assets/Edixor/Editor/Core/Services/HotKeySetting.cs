using HotKeySettingsBase = EdixorSettingsData<HotKeySettingsAsset, KeyActionData, HotKeyId>;
using SettingEntries = ISettingEntries<HotKeySettingsAsset.KeyActionDictionaryEntry, string>;
using SettingItemFull = ISettingItemFull<KeyAction, HotKeyId>;

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ExTools;
using System;

public class HotKeySetting : HotKeySettingsBase, SettingEntries, SettingItemFull
{
    private DIContainer container;
    public HotKeySetting(DIContainer container) : base("SettingAsset/HotKeySettings.asset") {
        this.container = container;
    }

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
            Settings.Entries.Add(new HotKeySettingsAsset.KeyActionDictionaryEntry {
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

        if (!Settings.HotKeyDictionary.TryGetValue(key.title, out var dataArray))
        {
            ExDebug.LogWarning($"[HotKeySetting] Не найдена категория хоткеев с заголовком: \"{key.title}\"");
            return null;
        }

        var data = dataArray.FirstOrDefault(d => d.Name.Equals(key.name, StringComparison.OrdinalIgnoreCase));
        if (data == null)
        {
            ExDebug.LogWarning($"[HotKeySetting] В категории \"{key.title}\" не найдена горячая клавиша с именем: \"{key.name}\"");
            return null;
        }

        try
        {
            KeyActionLogic logic = container.ResolveNamed<KeyActionLogic>(data.LogicKey);
            return new KeyAction(data, logic);
        }
        catch (Exception e)
        {
            ExDebug.LogError($"[HotKeySetting] Не удалось разрешить логику \"{data.LogicKey}\" для клавиши \"{key.name}\" в \"{key.title}\". Ошибка: {e.Message}");
            return null;
        }
    }


    public KeyAction[] GetAllItemFull()
    {
        EnsureDictionaryIsUpToDate();

        return Settings.HotKeyDictionary
            .SelectMany(kvp => kvp.Value)
            .Select(data => new KeyAction(data, container.ResolveNamed<KeyActionLogic>(data.LogicKey)))
            .ToArray();

    }

    public HotKeySettingsAsset.KeyActionDictionaryEntry GetEntries(string key)
    {
        EnsureDictionaryIsUpToDate();

        return Settings.Entries.FirstOrDefault(e => e.Key == key);
    }
    
    public HotKeySettingsAsset.KeyActionDictionaryEntry GetEntry(string key)
    {
        EnsureDictionaryIsUpToDate();
        return Settings.Entries.FirstOrDefault(e => e.Key == key);
    }

    public IReadOnlyList<HotKeySettingsAsset.KeyActionDictionaryEntry> GetAllEntries()
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

    public void UpdateEntry(string key, HotKeySettingsAsset.KeyActionDictionaryEntry updated)
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