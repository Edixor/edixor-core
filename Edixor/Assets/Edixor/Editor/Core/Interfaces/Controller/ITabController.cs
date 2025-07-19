namespace ExTools
{
    public interface ITabController
    {
        void Initialize(IUIController uiBase = null, TabSetting tabSetting = null);
        void RestoreTabs();
        void AddTab(TabData newTab, bool saveState = true, bool autoSwitch = true);
        void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true);
        void AddTab(string tabName, bool saveState = true, bool autoSwitch = true);
        void SwitchTab(int index);
        void CloseTab(int index);
        void CloseAllTabs();
        void OnGUI();
        void OnWindowClose();
        void SetBasicTab(TabData tab);
        EdixorTab GetLogicFor(TabData data);
    }
}
