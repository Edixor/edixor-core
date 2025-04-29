using UnityEngine;
using UnityEditor;

public abstract class EdixorSettingsData<TData, TItem, IKey> : EdixorSettingData<TData> 
    where TData : ScriptableObject
    where TItem : class
{
    public EdixorSettingsData(string path) : base(path) { }
    public abstract void AddItem(TItem item, IKey key);
    public abstract void RemoveItem(IKey key);
    public abstract void UpdateItem(IKey key, TItem item);
    public abstract TItem GetItem(IKey key);
    public abstract TItem[] GetAllItem(); 
}
