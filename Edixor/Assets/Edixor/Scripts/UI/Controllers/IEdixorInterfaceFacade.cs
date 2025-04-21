using UnityEngine.UIElements;
public interface IEdixorInterfaceFacade
{
    void InitOptions(VisualElement root, DIContainer container);
    void Initialize();
    void AddHotKey(KeyAction key);
    void LoadHotKey(string path, System.Action action = null);
    void OnKeys();
    void Update();
    void OnWindowClose();
    void AddTab(EdixorTab tab, bool saveState = true, bool autoSwitch = true);
}
