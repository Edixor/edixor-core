using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "HotKeySettings", menuName = "Edixor/HotKeySettings", order = 1)]
public class HotKeySaveAsset : ScriptableObject
{
    [SerializeField]
    private List<KeyActionDictionaryEntry> hotKeyDictionaryEntries = new();

    private Dictionary<string, KeyActionData[]> hotKeyDictionary;

    public Dictionary<string, KeyActionData[]> HotKeyDictionary
    {
        get
        {
            if (hotKeyDictionary == null)
            {
                RebuildDictionaryFromList();
            }
            return hotKeyDictionary;
        }
    }

    public List<KeyActionDictionaryEntry> Entries
    {
        get => hotKeyDictionaryEntries;
        set
        {
            hotKeyDictionaryEntries = value ?? new List<KeyActionDictionaryEntry>();
            RebuildDictionaryFromList();
        }
    }

    public void RebuildDictionaryFromList()
    {
        hotKeyDictionary = new Dictionary<string, KeyActionData[]>();

        foreach (var entry in hotKeyDictionaryEntries)
        {
            if (!string.IsNullOrEmpty(entry.Key))
            {
                hotKeyDictionary[entry.Key] = entry.Values;
            }
        }
    }

    [System.Serializable]
    public class KeyActionDictionaryEntry
    {
        public string Key;
        public KeyActionData[] Values;

        // Добавляем необходимые свойства
        public bool enable = true;
        public string Name => Key;

        // Если KeyActionData содержит набор комбинаций клавиш, можно добавить:
        public IEnumerable<string> Combination
        {
            get
            {
                if (Values != null && Values.Length > 0)
                {
                    return Values.Select(v => v.ToString());
                }
                return new string[0];
            }
        }
    }
}
