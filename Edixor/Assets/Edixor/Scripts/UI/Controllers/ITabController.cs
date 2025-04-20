public interface ITabController
{
    void Initialize(IUIController uiBase = null, TabService tabService = null);
    void RestoreTabs();
    void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true);
    void SwitchTab(int index);
    void CloseTab(int index);
    void CloseAllTabs();
    void OnGUI();
    void OnWindowClose();
}
