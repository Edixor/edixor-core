using System.Collections.Generic;
public interface ISettingEntries<TEntry, TKey>
{
    TEntry GetEntries(TKey key);
    IReadOnlyList<TEntry> GetAllEntries();
    void RemoveEntry(TKey key);
    void UpdateEntry(TKey key, TEntry entry);
}
