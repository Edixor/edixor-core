using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine;
using ExTools;

public class TabController : ITabController
{
    private readonly DIContainer container;
    private readonly TabUIFactory tabUIFactory;
    private IUIController uiBase;
    private TabSetting tabSetting;
    private List<EdixorTab> tabs = new();
    private Dictionary<EdixorTab, VisualElement> tabContainers = new();
    private int indexTab = -1;

    public TabController(DIContainer container)
    {
        this.container = container;
        this.tabUIFactory = new TabUIFactory();
    }

    public void Initialize(IUIController uiBase = null, TabSetting tabSetting = null)
    {
        this.uiBase = uiBase ?? container.ResolveNamed<IUIController>(ServiceNames.UIController);
        this.tabSetting = tabSetting ?? container.ResolveNamed<TabSetting>(ServiceNames.TabSetting);
    }

    public void RestoreTabs()
    {
        ClearAll();
        var savedTabs = tabSetting.GetTabs();
        if (savedTabs == null || savedTabs.Count == 0)
        {
            AddTab(new NewTab(), false, true);
            return;
        }
        foreach (var tab in savedTabs)
            AddTab(tab, false, false);
        int savedIndex = Mathf.Clamp(tabSetting.GetActiveTab(), 0, tabs.Count - 1);
        SwitchTab(savedIndex);
    }

    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null) return;
        tabs.Add(newTab);
        if (saveState)
            tabSetting.SetTabs(tabs);

        try
        {
            newTab.Initialize(uiBase.GetElement("middle-section-content"), container);
        }
        catch (System.Exception ex)
        {
            ExDebug.LogError($"[TabController] Error initializing tab \"{newTab.Title}\": {ex}");
        }
        if (autoSwitch)
            SwitchTab(tabs.IndexOf(newTab));

        var tabContainer = tabUIFactory.CreateTabContainer(
            newTab,
            () => SwitchTab(tabs.IndexOf(newTab)),
            () => CloseTab(tabs.IndexOf(newTab))
        );

        tabContainers[newTab] = tabContainer;
        uiBase.GetElement("tab-section").Add(tabContainer);
    }

    public void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;
        if (indexTab >= 0 && indexTab < tabs.Count)
            tabs[indexTab].InvokeOnDisable();
        indexTab = index;
        foreach (var kv in tabContainers)
            kv.Value.EnableInClassList("active", kv.Key == tabs[indexTab]);
        var contentRoot = uiBase.GetElement("middle-section-content");
        contentRoot.Clear();
        var current = tabs[indexTab];
        try
        {
            current.Initialize(contentRoot, container);
            current.InvokeAwake();
            current.InvokeStart();
            current.InvokeOnEnable();
        }
        catch (System.Exception ex)
        {
            ExDebug.LogError($"[TabController] Error displaying tab \"{current.Title}\": {ex}");
            var errorLabel = new Label($"🔴 Tab error: {ex.Message}");
            errorLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            contentRoot.Clear();
            contentRoot.Add(errorLabel);
        }
        tabSetting.SetLastActiveTabIndex(indexTab);
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;
        var closingTab = tabs[index];
        closingTab.InvokeOnDisable();
        closingTab.InvokeOnDestroy();
        closingTab.DeleteUI();
        if (tabContainers.TryGetValue(closingTab, out var containerElement))
        {
            uiBase.GetElement("tab-section").Remove(containerElement);
            tabContainers.Remove(closingTab);
        }
        tabs.RemoveAt(index);
        tabSetting.SetTabs(tabs);
        if (tabs.Count == 0)
        {
            indexTab = -1;
            uiBase.Show(new EmptyUI());
            return;
        }
        int newIndex = Mathf.Clamp(indexTab == index ? index - 1 : indexTab, 0, tabs.Count - 1);
        SwitchTab(newIndex);
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
        tabContainers.Clear();
        indexTab = -1;
        uiBase.Show(new EmptyUI());
    }

    public void OnGUI()
    {
        if (indexTab >= 0 && indexTab < tabs.Count)
            tabs[indexTab].InvokeUpdateGUI();
    }

    public void OnWindowClose()
    {
        tabSetting.SetTabs(tabs);
    }

    private void ClearAll()
    {
        var tabSection = uiBase.GetElement("tab-section");
        tabSection.Clear();
        var middle = uiBase.GetElement("middle-section-content");
        middle.Clear();
        foreach (var tab in tabs)
            tab.DeleteUI();
        tabs.Clear();
        tabContainers.Clear();
        indexTab = -1;
    }
}
