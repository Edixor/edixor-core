using System;
public interface IFactoryHotKey
{
    void CreateHotKey(string keyOrPath, Action action = null);
}