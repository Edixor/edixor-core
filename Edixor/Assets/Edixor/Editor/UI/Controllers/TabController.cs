using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
            AddTab(new NewTab(), saveState: false, autoSwitch: true);
            return;
        }

        foreach (var tab in savedTabs)
            AddTab(tab, saveState: false, autoSwitch: false);

        int savedIndex = tabSetting.GetActiveTab();
        savedIndex = Mathf.Clamp(savedIndex, 0, tabs.Count - 1);
        SwitchTab(savedIndex);
    }

    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null) return;

        tabs.Add(newTab);
        if (saveState)
            tabSetting.SetTabs(tabs);

        // Создаём UI-кнопки вкладок
        var tabContainer = tabUIFactory.CreateTabContainer(
            newTab,
            onSwitch: () => SwitchTab(tabs.IndexOf(newTab)),
            onClose:   () => CloseTab(tabs.IndexOf(newTab))
        );
        tabContainers[newTab] = tabContainer;
        uiBase.GetElement("tab-section").Add(tabContainer);

        // **Всегда** сначала Awake (грузит UXML и создаёт root)
        newTab.Initialize(uiBase.GetElement("middle-section-content"), container);
        newTab.InvokeAwake();

        if (autoSwitch)
        {
            // повторно вызывает Awake/Start/OnEnable и подсветку таба
            SwitchTab(tabs.IndexOf(newTab));
        }
        else
        {
            // теперь root точно не null, Start безопасно выполнится
            newTab.InvokeStart();
        }
    }

    public void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        // Disable previous
        if (indexTab >= 0)
            tabs[indexTab].InvokeOnDisable();

        indexTab = index;
        var current = tabs[indexTab];

        // Update saved state
        tabSetting.SetLastActiveTabIndex(indexTab);

        // Highlight active tab
        foreach (var kv in tabContainers)
        {
            kv.Value.EnableInClassList("active", kv.Key == current);
        }

        // Initialize and show content
        var contentRoot = uiBase.GetElement("middle-section-content");
        contentRoot.Clear();
        current.Initialize(contentRoot, container);
        current.InvokeAwake();
        current.InvokeStart();
        current.InvokeOnEnable();
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        var closingTab = tabs[index];
        closingTab.InvokeOnDisable();
        closingTab.InvokeOnDestroy();
        closingTab.DeleteUI();

        // Remove UI
        if (tabContainers.TryGetValue(closingTab, out var container))
        {
            uiBase.GetElement("tab-section").Remove(container);
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
        // clear UI section
        var tabSection = uiBase.GetElement("tab-section");
        tabSection.Clear();
        var middle = uiBase.GetElement("middle-section-content");
        middle.Clear();

        // clear lists
        foreach (var tab in tabs)
            tab.DeleteUI();

        tabs.Clear();
        tabContainers.Clear();
        indexTab = -1;
    }
}
