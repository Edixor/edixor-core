using System.Collections.Generic; 
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorUIManager
{
    private List<EdixorTab> tabs = new List<EdixorTab>();
    private int indexTab = -1;
    private Dictionary<EdixorTab, VisualElement> tabContainers = new Dictionary<EdixorTab, VisualElement>();

    private readonly DIContainer container;
    private EdixorDesign design;
    private VisualElement root;

    private WindowStateService WindowStateService =>
        container.ResolveNamed<WindowStateService>(ServiceNames.WindowStateSetting);
    private TabService TabService =>
        container.ResolveNamed<TabService>(ServiceNames.TabSetting);
    private StyleService StyleService =>
        container.ResolveNamed<StyleService>(ServiceNames.StyleSetting);
    private LayoutService LayoutService =>
        container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting);

    public EdixorUIManager(DIContainer container)
    {
        this.container = container;
    }

    public void LoadUI()
    {
        root = WindowStateService.GetRootElement();
        design = new EdixorDesign(
            StyleService.GetCurrentItem(),
            LayoutService.GetCurrentItem(),
            root,
            container
        );
        design.LoadUI();

        if (!RestoreTabs() || tabs.Count == 0)
        {
            AddTab(new NewTab(), saveState: false, autoSwitch: true);
        }
        else
        {
            if (indexTab >= 0 && indexTab < tabs.Count)
            {
                tabs[indexTab].InvokeAwake();
                tabs[indexTab].Initialize(design.GetSection("middle-section-content"), container, tabs[indexTab].Title, tabs[indexTab].PathUxml, tabs[indexTab].PathUss);
                tabs[indexTab].InvokeOnEnable();
                tabs[indexTab].InvokeStart();
                tabs[indexTab].InvokeOnUI();
            }
        }
    }

    private bool RestoreTabs()
    {
        List<EdixorTab> savedTabs = TabService.GetTabs();
        if (savedTabs != null && savedTabs.Count > 0)
        {
            design.GetSection("tab-section").Clear();
            VisualElement middleSection = design.GetSection("middle-section-content");
            middleSection.Clear();

            foreach (EdixorTab tab in savedTabs)
            {
                tab.InvokeAwake();
                // Вместо старого tab.Init(...) вызываем Initialize.
                // Предполагается, что tab уже содержит корректные пути и имя.
                tab.Initialize(middleSection, container, tab.Title, tab.PathUxml, tab.PathUss);
                tab.InvokeStart();
                AddTab(tab, saveState: false, autoSwitch: false);
            }

            int savedActiveIndex = TabService.GetActiveTab();
            if (savedActiveIndex < 0 || savedActiveIndex >= tabs.Count)
                savedActiveIndex = 0;
            SwitchTab(savedActiveIndex);
            return true;
        }
        return false;
    }

    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null) return;

        ClearEmptyStateUI();

        newTab.InvokeAwake();
        tabs.Add(newTab);
        var tabContainer = CreateTabContainer(newTab);
        tabContainers[newTab] = tabContainer;
        design.GetSection("tab-section").Add(tabContainer);

        if (saveState)
            TabService.SetTabs(tabs);

        if (autoSwitch)
            SwitchTab(tabs.IndexOf(newTab));
        else
            newTab.InvokeStart();
    }

    private void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count)
            return;

        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].InvokeOnDisable();
        }
        
        tabs[index].InvokeAwake();
        tabs[index].Initialize(design.GetSection("middle-section-content"), container, tabs[index].Title, tabs[index].PathUxml, tabs[index].PathUss);
        tabs[index].InvokeStart();
        tabs[index].InvokeOnEnable();
        // После инициализации можно также вызвать InvokeOnUI() для обновления контента вкладки.
        tabs[index].InvokeOnUI();
        indexTab = index;
        TabService.SetLastActiveTabIndex(index);
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        var closingTab = tabs[index];
        closingTab.InvokeOnDisable();
        closingTab.InvokeOnDestroy();
        closingTab.DeleteUI();

        if (tabContainers.TryGetValue(closingTab, out var tabContainer))
        {
            design.GetSection("tab-section").Remove(tabContainer);
            tabContainers.Remove(closingTab);
        }

        bool wasActiveTabClosed = (indexTab == index);
        tabs.RemoveAt(index);

        TabService.SetTabs(tabs);

        if (tabs.Count == 0)
        {
            indexTab = -1;
            ShowEmptyStateUI();
            return;
        }

        if (wasActiveTabClosed)
        {
            int newActiveIndex = index > 0 ? index - 1 : 0;
            SwitchTab(newActiveIndex);
        }
        else
        {
            if (indexTab > index)
                indexTab--;

            SwitchTab(indexTab);
        }
    }

    // Заглушка: очистка UI для состояния "нет вкладок"
    private void ClearEmptyStateUI()
    {
        // Реализуйте очистку секции, если необходимо.
    }

    // Заглушка: создание контейнера для вкладки
    private VisualElement CreateTabContainer(EdixorTab tab)
    {
        // Можно создать и вернуть новый VisualElement с необходимой разметкой для вкладки.
        return new VisualElement();
    }

    // Заглушка: показать UI пустого состояния
    private void ShowEmptyStateUI()
    {
        // Реализуйте показ UI, если нет вкладок.
    }

    public EdixorDesign GetDesign()
    {
        return design;
    }
}
