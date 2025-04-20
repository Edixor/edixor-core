using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class TabController : ITabController
{
    private readonly DIContainer container;
    private readonly TabFactory tabFactory;
    private IUIController uiBase;
    private TabService tabService;

    private List<EdixorTab> tabs = new();
    private Dictionary<EdixorTab, VisualElement> tabContainers = new();
    private int indexTab = -1;

    public TabController(DIContainer container)
    {
        this.container = container;
        this.tabFactory = new TabFactory();
    }

    public void Initialize(IUIController uiBase = null, TabService tabService = null) {
        this.uiBase = uiBase ?? container.ResolveNamed<IUIController>(ServiceNames.UIController);
        this.tabService = tabService ?? container.ResolveNamed<TabService>(ServiceNames.TabSetting);
    }

    public void RestoreTabs()
    {
        tabs.Clear();
        tabContainers.Clear();

        var savedTabs = tabService.GetTabs();
        if (savedTabs == null || savedTabs.Count == 0)
        {
            AddTab(new NewTab(), saveState: false, autoSwitch: true);
            return;
        }

        VisualElement tabSection = uiBase.GetElement("tab-section");
        while (tabSection.childCount > 1)
        {
            tabSection.RemoveAt(1);
        }

        VisualElement middle = uiBase.GetElement("middle-section-content");
        middle.Clear();

        foreach (var tab in savedTabs)
        {
            AddTab(tab, saveState: false, autoSwitch: false);
        }

        int savedIndex = tabService.GetActiveTab();
        if (savedIndex < 0 || savedIndex >= tabs.Count)
            savedIndex = 0;

        SwitchTab(savedIndex);
    }

    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null) return;

        newTab.Initialize(uiBase.GetElement("middle-section-content"), container);
        tabs.Add(newTab);

        if (saveState)
            tabService.SetTabs(tabs);

        if (autoSwitch)
            SwitchTab(tabs.IndexOf(newTab));
        else
            newTab.InvokeStart();
    }

    public void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count)
            return;

        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].InvokeOnDisable();
        }

        var newTab = tabs[index];
        newTab.InvokeAwake();
        newTab.Initialize(uiBase.GetElement("middle-section-content"), container);
        newTab.InvokeStart();
        newTab.InvokeOnEnable();

        var tabContainer = tabFactory.CreateTabContainer(
            newTab,
            onSwitch: () => SwitchTab(tabs.IndexOf(newTab)),
            onClose: () => CloseTab(tabs.IndexOf(newTab))
        );

        tabContainers[newTab] = tabContainer;
        uiBase.GetElement("tab-section").Add(tabContainer);

        indexTab = index;
        tabService.SetLastActiveTabIndex(index);
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count)
            return;

        var closingTab = tabs[index];
        closingTab.InvokeOnDisable();
        closingTab.InvokeOnDestroy();
        closingTab.DeleteUI();

        if (tabContainers.TryGetValue(closingTab, out var tabContainer))
        {
            uiBase.GetElement("tab-section").Remove(tabContainer);
            tabContainers.Remove(closingTab);
        }

        bool wasActive = index == indexTab;
        tabs.RemoveAt(index);
        tabService.SetTabs(tabs);

        if (tabs.Count == 0)
        {
            indexTab = -1;
            uiBase.Show(new EmptyUI());
            return;
        }

        if (wasActive)
        {
            int newIndex = index > 0 ? index - 1 : 0;
            SwitchTab(newIndex);
        }
        else if (indexTab > index)
        {
            indexTab--;
        }

        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            SwitchTab(indexTab);
        }
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
        uiBase.Show(new EmptyUI());
    }

    public void OnGUI()
    {
        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].InvokeUpdateGUI();
        }
    }

    public void OnWindowClose()
    {
        tabService.SetTabs(tabs);
    }
}
