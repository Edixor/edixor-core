using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using ExTools;

public class TabController : ITabController
{
    private const int MaxTabs = 50;
    private readonly DIContainer _container;
    private readonly TabUIFactory tabUIFactory;
    private IUIController uiBase;
    private TabSetting tabSetting;
    private List<EdixorTab> tabs = new List<EdixorTab>();
    private Dictionary<EdixorTab, VisualElement> tabContainers = new Dictionary<EdixorTab, VisualElement>();
    private int indexTab = -1;

    public TabController(DIContainer container)
    {
        _container = container;
        tabUIFactory = new TabUIFactory();
    }

    public void Initialize(IUIController uiBase = null, TabSetting tabSetting = null)
    {
        this.uiBase = uiBase ?? _container.ResolveNamed<IUIController>(ServiceNames.UIController);
        this.tabSetting = tabSetting ?? _container.ResolveNamed<TabSetting>(ServiceNames.TabSetting);
    }

    public void RestoreTabs()
    {
        ExDebug.BeginGroup("TabController: restore tabs");
        ClearAll();
        var savedTabs = tabSetting.GetTabs() ?? new List<EdixorTab>();
        if (savedTabs.Count == 0)
        {
            AddTab(new NewTab(), false, true);
        }
        else
        {
            foreach (var tab in savedTabs)
                AddTab(tab, false, false);
            int savedIndex = Mathf.Clamp(tabSetting.GetActiveTab(), 0, tabs.Count - 1);
            SwitchTab(savedIndex);
        }
        ExDebug.EndGroup();
    }

    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null)
            return;

        if (tabs.Count >= MaxTabs)
        {
            ExDebug.LogWarning($"[TabController] Превышен лимит вкладок ({MaxTabs}). Новая вкладка не будет добавлена.");
            return;
        }

        tabs.Add(newTab);
        if (saveState)
            tabSetting.SetTabs(tabs);

        TryInitializeTab(newTab);
        if (autoSwitch)
            SwitchTab(tabs.IndexOf(newTab));

        var container = tabUIFactory.CreateTabContainer(
            newTab,
            GetStyleParameters(),
            () => SwitchTab(tabs.IndexOf(newTab)),
            () => CloseTab(tabs.IndexOf(newTab))
        );

        tabContainers[newTab] = container;
        uiBase.GetElement("tab-section").Add(container);
    }

    public void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count)
            return;

        if (indexTab >= 0 && indexTab < tabs.Count)
            tabs[indexTab].InvokeOnDisable();

        indexTab = index;
        var styleParams = GetStyleParameters();

        foreach (var kv in tabContainers)
        {
            bool isActive = kv.Key == tabs[indexTab];
            styleParams.TabStyle.ApplyWithStates(kv.Value.Q<Button>("tab-button"));
            styleParams.TabStyle.ApplyWithStates(kv.Value.Q<Button>("tab-button-exit"));
        }

        RefreshContent(tabs[indexTab]);
        tabSetting.SetLastActiveTabIndex(indexTab);
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count)
            return;

        var closingTab = tabs[index];
        closingTab.InvokeOnDisable();
        closingTab.InvokeOnDestroy();
        closingTab.DeleteUI();

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
        }
        else
        {
            int newIndex = Mathf.Clamp(indexTab == index ? index - 1 : indexTab, 0, tabs.Count - 1);
            SwitchTab(newIndex);
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

    private void RefreshContent(EdixorTab current)
    {
        var root = uiBase.GetElement("middle-section-content");
        root.Clear();
        try
        {
            current.Initialize(root, _container);
            current.InvokeAwake();
            current.InvokeStart();
            current.InvokeOnEnable();
        }
        catch (System.Exception ex)
        {
            ShowError(root, ex, current.Title);
        }
    }

    private void ClearAll()
    {
        // Удаляем UI, сбрасываем состояние списка
        foreach (var kv in tabContainers)
            kv.Key.DeleteUI();

        tabs.Clear();
        tabContainers.Clear();
        indexTab = -1;

        // Очищаем визуальные контейнеры, но сохраняем кнопку добавления
        var tabSection = uiBase.GetElement("tab-section");
        var addBtn = tabSection.Q<Button>("AddTab");
        EdixorParameters p = GetStyleParameters();
        tabSection.Clear();
        if (addBtn != null)
            p.AddTabStyle.ApplyWithStates(addBtn);
            tabSection.Add(addBtn);

        uiBase.GetElement("middle-section-content").Clear();
    }

    private EdixorParameters GetStyleParameters()
    {
        return ((EdixorParameters)_container
            .ResolveNamed<StyleSetting>(ServiceNames.StyleSetting)
            .GetCorrectItem()
            .AssetParameters[0]);
    }

    private void TryInitializeTab(EdixorTab tab)
    {
        try
        {
            tab.Initialize(uiBase.GetElement("middle-section-content"), _container);
        }
        catch (System.Exception ex)
        {
            ExDebug.LogError($"[TabController] Error initializing tab '{tab.Title}': {ex}");
        }
    }

    private void ShowError(VisualElement parent, System.Exception ex, string title)
    {
        string fullError = ex.ToString();
        ExDebug.LogError($"[TabController] Error displaying tab '{title}': {fullError}");

        var content = new VisualElement { style = { flexDirection = FlexDirection.Column } };
        var label = new Label($"🔴 Tab error:\n{fullError}") { style = { whiteSpace = WhiteSpace.Normal } };
        var copyBtn = new Button(() => { GUIUtility.systemCopyBuffer = fullError; ExDebug.Log("Copied error"); }) { text = "📋 Копировать" };
        content.Add(copyBtn);
        content.Add(label);
        parent.Clear(); parent.Add(content);
    }
}
