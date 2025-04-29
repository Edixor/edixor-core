using System;
public interface IFactoryHotKey
{
    void CreateFromAssets(string path, HotKeyId key, Action action = null);
    void CreateExample(HotKeyExampleData data, HotKeyId key);
}
