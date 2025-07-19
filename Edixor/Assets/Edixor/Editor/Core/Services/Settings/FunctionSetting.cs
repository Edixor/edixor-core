using FunctionSettingsBase = EdixorSettingsData<FunctionSettingsAsset, FunctionData, string>;
using SettingItemFull = ISettingItemFull<Function, string>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

using ExTools;

namespace ExTools.Settings
{
    public class FunctionSetting : FunctionSettingsBase, SettingItemFull
    {
        private readonly DIContainer _container;

        public FunctionSetting(DIContainer container, string edixorId)
            : base($"WindowStates/{edixorId}", "FunctionSettings.asset")
        {
            this._container = container;
        }

        public override FunctionData[] GetAllItem() => Settings.Items.ToArray();

        public Function[] GetAllItemFull()
        {
            var functions = new List<Function>();

            foreach (var item in Settings.Items)
            {
                if (item == null) continue;

                var logic = new ExFunction(item.ScriptLogic, _container);
                var logicInstance = logic.GetLogic();

                if (logicInstance == null)
                {
                    ExDebug.LogWarning($"Failed to create logic for function: {item.Name}");
                    continue;
                }

                functions.Add(new Function(item, logicInstance));
            }

            return functions.ToArray();
        }

        public override FunctionData GetItem(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Function name cannot be null or empty.", nameof(key));

            var function = Settings.Items
                .FirstOrDefault(item => item != null && item.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (function == null)
                throw new InvalidOperationException($"Function with name '{key}' was not found.");

            return function;
        }

        public Function GetItemFull(string key)
        {
            var data = GetItem(key);

            var logic = new ExFunction(data.ScriptLogic, _container);
            var logicInstance = logic.GetLogic();

            return new Function(data, logicInstance);
        }

        public override void AddItem(FunctionData item, string key)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (string.IsNullOrEmpty(item.Name))
                throw new ArgumentException("Function name cannot be null or empty.", nameof(item));

            var list = Settings.Items ?? new List<FunctionData>();

            if (list.Any(i => i?.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase) == true))
            {
                var setting = _container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting);
                bool coldStart = setting?.GetCorrectItem()?.IsColdStart ?? true;

                string msg = $"Function with name '{item.Name}' already exists — skipping.";
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
                throw new ArgumentException("Function name cannot be null or empty.", nameof(key));

            var function = Settings.Items
                .FirstOrDefault(item => item.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (function == null)
                throw new InvalidOperationException($"Function with name '{key}' was not found.");

            Settings.Items.Remove(function);
            SaveSettings();
        }

        public override void UpdateItem(string key, FunctionData item)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Function name cannot be null or empty.", nameof(key));

            var index = Settings.Items
                .FindIndex(i => i.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (index == -1)
                throw new InvalidOperationException($"Function with name '{key}' was not found.");

            Settings.Items[index] = item;
            SaveSettings();
        }
    }
}
