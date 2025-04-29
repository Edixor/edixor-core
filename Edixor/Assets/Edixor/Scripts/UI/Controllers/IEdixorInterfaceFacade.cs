using UnityEngine.UIElements;
using System;
public interface IEdixorInterfaceFacade
{
    void InitOptions(VisualElement root, DIContainer container);
    void Initialize();
    void AddHotKey(KeyAction key);
    void LoadHotKey(string path, string title = null, Action action = null);
    void LoadFunction(string path, string container = null, Action action = null);
    void OnKeys();
    void Update();
    void OnWindowClose();
    void AddTab(EdixorTab tab, bool saveState = true, bool autoSwitch = true);
}
