using System;
public interface IFactoryStatus
{
    void InitializeData(StatusSetting settingsData, StatusController controller);
    void CreateExample(string data, string key);
    void CreateFromAssets(string path, string key = null, Action action = null);
}