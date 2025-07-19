using EdixorRegistrySettingBase = EdixorSettingsData<EdixorRegistryAsset, EdixorRegistryEntry, string>;
using SettingCorrectItem = ISettingCorrectItem<LayoutData>;

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using ExTools;

namespace ExTools.Settings
{
    public class EdixorRegistrySetting : EdixorRegistrySettingBase
    {
        private string _lastFocusedId;

        public EdixorRegistrySetting()
            : base("SettingAsset", "EdixorRegistrySetting.asset")
        { }

        public override void AddItem(EdixorRegistryEntry item, string key = null)
        {
            Debug.Log(0);
            Debug.Log(item.Id);
            if (item == null) return;

            if (Settings.Items.Exists(x => x.Id.Equals(item.Id, System.StringComparison.OrdinalIgnoreCase)))
                return;

            Debug.Log(1);
            Settings.Items.Add(item);
            SaveSettings();
        }

        public override void RemoveItem(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            var item = GetItem(key);
            if (item == null) return;

            Settings.Items.Remove(item);
            SaveSettings();
        }

        public override void UpdateItem(string key, EdixorRegistryEntry item)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (item == null) return;

            var existingItem = GetItem(key);
            if (existingItem == null) return;

            int index = Settings.Items.IndexOf(existingItem);
            Settings.Items[index] = item;
            SaveSettings();
        }

        public override EdixorRegistryEntry GetItem(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return Settings.Items.Find(x => x.Id.Equals(key, System.StringComparison.OrdinalIgnoreCase));
        }

        public override EdixorRegistryEntry[] GetAllItem()
        {
            return Settings.Items.ToArray();
        }

        public EdixorRegistryEntry GetCorrectItem()
        {
            if (string.IsNullOrEmpty(_lastFocusedId))
            {
                Debug.LogWarning("GetCorrectItem: _lastFocusedId is null or empty.");
                return null;
            }

            var item = GetItem(_lastFocusedId);
            if (item == null)
            {
                Debug.LogWarning($"GetCorrectItem: No item found with Id '{_lastFocusedId}'.");
            }
            return item;
        }

        public void SetCorrectItem(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning("SetCorrectItem: key is null or empty.");
                return;
            }

            var item = GetItem(key);
            if (item == null)
            {
                Debug.LogWarning($"SetCorrectItem: No item found with Id '{key}'.");
                return;
            }

            _lastFocusedId = item.Id;

            foreach (var itemEntry in Settings.Items)
            {
                itemEntry.Focus = itemEntry.Id.Equals(_lastFocusedId, System.StringComparison.OrdinalIgnoreCase);
            }
        }

        public bool IsRegistered(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            return Settings.Items.Exists(x => x.Id.Equals(key, System.StringComparison.OrdinalIgnoreCase));
        }

        public void SetColdStart(bool value)
        {
            foreach (EdixorRegistryEntry item in Settings.Items)
            {
                item.IsColdStart = value;
            }
            SaveSettings();
        }
    }
}
