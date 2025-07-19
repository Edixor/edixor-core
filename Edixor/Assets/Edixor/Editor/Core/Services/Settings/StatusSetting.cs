using StatusSettingsBase = EdixorSettingsData<StatusSettingsAsset, StatusData, string>;
using SettingItemFull = ISettingItemFull<Status, string>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

using ExTools.Settings;
using ExTools;

public class StatusSetting : StatusSettingsBase, SettingItemFull
{
    private readonly DIContainer _container;

    public StatusSetting(DIContainer container, string edixorId)
        : base($"WindowStates/{edixorId}", "StatusSettings.asset")
    {
        this._container = container;
    }

    public override StatusData[] GetAllItem() => Settings.Items.ToArray();

    public Status[] GetAllItemFull()
    {
        var statuses = new List<Status>();

        foreach (var item in Settings.Items)
        {
            if (item == null) continue;

            var logic = new ExStatus(item.ScriptLogic, _container);
            var logicInstance = logic.GetLogic();

            if (logicInstance == null)
            {
                ExDebug.LogWarning($"Failed to create logic for status: {item.Name}");
                continue;
            }

            statuses.Add(new Status(item, logicInstance));
        }

        return statuses.ToArray();
    }

    public override StatusData GetItem(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Status name cannot be null or empty.", nameof(key));

        var status = Settings.Items
            .FirstOrDefault(item => item != null && item.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (status == null)
            throw new InvalidOperationException($"Status with name '{key}' not found.");

        return status;
    }

    public Status GetItemFull(string key)
    {
        var data = GetItem(key);

        var logic = new ExStatus(data.ScriptLogic, _container);
        var logicInstance = logic.GetLogic();

        return new Status(data, logicInstance);
    }

    public override void AddItem(StatusData item, string key)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));
        if (string.IsNullOrEmpty(item.Name))
            throw new ArgumentException("Status name cannot be null or empty.", nameof(item));

        var list = Settings.Items ?? new List<StatusData>();

        if (list.Any(i => i?.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase) == true))
        {
            var setting = _container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting);
            bool coldStart = setting?.GetCorrectItem()?.IsColdStart ?? true;

            string msg = $"Status with name '{item.Name}' already exists — skipping addition.";
            if (coldStart) ExDebug.Log(msg);
            else ExDebug.LogWarning(msg);
            return;
        }

        if (!string.IsNullOrEmpty(key) && item.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
        {
            list.Add(item);
        }
        else if (string.IsNullOrEmpty(key))
        {
            list.Add(item);
        }
        else
        {
            int idx = list.FindIndex(i => i?.Name.Equals(key, StringComparison.OrdinalIgnoreCase) == true);
            if (idx >= 0)
                list.Insert(idx + 1, item);
            else
                list.Add(item);
        }

        Settings.Items = list;
        SaveSettings();
    }

    public override void RemoveItem(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Status name cannot be null or empty.", nameof(key));

        var status = Settings.Items
            .FirstOrDefault(item => item.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (status == null)
            throw new InvalidOperationException($"Status with name '{key}' not found.");

        Settings.Items.Remove(status);
        SaveSettings();
    }

    public override void UpdateItem(string key, StatusData item)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Status name cannot be null or empty.", nameof(key));

        var index = Settings.Items
            .FindIndex(i => i.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
            throw new InvalidOperationException($"Status with name '{key}' not found.");

        Settings.Items[index] = item;
        SaveSettings();
    }
}
