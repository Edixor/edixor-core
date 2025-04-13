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

    // Фабрика для создания UI вкладок
    private TabFactory tabFactory;

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
        // Инициализация фабрики (можно внедрить через DI, если необходимо)
        this.tabFactory = new TabFactory();
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
                SwitchTab(indexTab);
            }
        }
    }

    private bool RestoreTabs()
    {
        tabs.Clear();
        tabContainers.Clear();

        List<EdixorTab> savedTabs = TabService.GetTabs();
        if (savedTabs != null && savedTabs.Count > 0)
        {
            VisualElement tabSection = design.GetSection("tab-section");
        while (tabSection.childCount > 1)
        {
            tabSection.RemoveAt(1);
        }


            VisualElement middleSection = design.GetSection("middle-section-content");
            middleSection.Clear();

            foreach (EdixorTab tab in savedTabs)
            {
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

        newTab.Initialize(design.GetSection("middle-section-content"), container);
        newTab.InvokeAwake();
        tabs.Add(newTab);

        // Используем фабрику для создания контейнера вкладки
        VisualElement tabContainer = tabFactory.CreateTabContainer(
            newTab,
            onSwitch: () => SwitchTab(tabs.IndexOf(newTab)),
            onClose: () => CloseTab(tabs.IndexOf(newTab))
        );
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
        tabs[index].Initialize(design.GetSection("middle-section-content"), container);
        tabs[index].InvokeStart();
        tabs[index].InvokeOnEnable();
  

        indexTab = index;
        TabService.SetLastActiveTabIndex(index);
    }

    public void CloseAllTabs()
    {
        foreach (var tab in tabs)
        {
            tab.InvokeOnDisable();
            tab.InvokeOnDestroy();
            tab.DeleteUI();
        }

        tabs.Clear();
        indexTab = -1;
        ShowEmptyStateUI();
    }

    public void OnGUI()
    {
        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].InvokeUpdateGUI();
        }
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

    private void ClearEmptyStateUI()
    {
        // Реализуйте очистку UI для пустого состояния, если необходимо.
    }

    private void ShowEmptyStateUI()
    {
        // Реализуйте отображение UI для пустого состояния, если необходимо.
    }

    public void OnWindowClose()
    {
        TabService.SetTabs(tabs);
    }

    public EdixorDesign GetDesign()
    {
        return design;
    }
}
