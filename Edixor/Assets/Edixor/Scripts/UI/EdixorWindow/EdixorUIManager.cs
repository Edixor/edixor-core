using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorUIManager
{
    private List<EdixorTab> tabs = new List<EdixorTab>();
    private int indexTab = -1;
    private Dictionary<EdixorTab, VisualElement> tabContainers = new Dictionary<EdixorTab, VisualElement>();

    private readonly EdixorWindow window;
    private EdixorDesign design;

    public EdixorUIManager(EdixorWindow window)
    {
        this.window = window;
    }

    public void LoadUI()
    {
        design = window.GetSetting().GetCurrentDesign();
        design.LoadUI();
        
        // Восстанавливаем ранее сохранённые вкладки, если они есть
        if (!RestoreTabs())
        {
            // Если вкладок не было сохранено – создаём первую вкладку по умолчанию
            AddTab(new NewTab(design.GetSection("middle-section-content")));
        }
    }

    /// <summary>
    /// Восстанавливает сохранённые вкладки из настроек.
    /// Возвращает true, если были восстановлены хотя бы одна вкладка.
    /// </summary>
    private bool RestoreTabs()
    {
        List<EdixorTab> savedTabs = window.GetSetting().GetTabs();
        if (savedTabs != null && savedTabs.Count > 0)
        {
            // Очищаем UI секцию вкладок и центральную область для корректного восстановления
            design.GetSection("tab-section").Clear();
            VisualElement middleSection = design.GetSection("middle-section-content");
            middleSection.Clear();
            
            // Восстанавливаем каждую вкладку
            foreach (EdixorTab tab in savedTabs)
            {
                tab.SetWindow(window);
                tab.SetParentContainer(middleSection);
                tab.Init();
                // Добавляем вкладку без повторного сохранения
                AddTab(tab, saveState: false);
            }
            
            // Определяем сохранённый индекс активной вкладки
            int savedActiveIndex = window.GetSetting().GetLastActiveTabIndex();
            if (savedActiveIndex < 0 || savedActiveIndex >= tabs.Count)
                savedActiveIndex = 0;
            SwitchTab(savedActiveIndex);
            return true;
        }
        return false;
    }


    // Перегруженный метод AddTab с возможностью отключить сохранение состояния
    public void AddTab(EdixorTab newTab, bool saveState = true)
    {
        if (newTab == null) return;

        // Если ранее отображался empty state, очищаем центральную область
        ClearEmptyStateUI();

        tabs.Add(newTab);
        var container = CreateTabContainer(newTab);
        tabContainers[newTab] = container;
        design.GetSection("tab-section").Add(container);
        
        // При добавлении новой вкладки сохраняем обновлённое состояние, если это не восстановление из настроек
        if (saveState)
            window.GetSetting().SetTabs(tabs);

        // Переключаемся на вновь добавленную вкладку
        SwitchTab(tabs.IndexOf(newTab));
    }

    // Создание контейнера для вкладки с кнопками переключения и закрытия
    private VisualElement CreateTabContainer(EdixorTab tab)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        container.style.marginRight = 4; // отступ между вкладками

        // Кнопка для переключения на вкладку
        var tabButton = new Button(() => SwitchTab(tabs.IndexOf(tab)))
        {
            text = tab.Title
        };
        container.Add(tabButton);

        // Кнопка для закрытия вкладки ("крестик")
        var closeButton = new Button(() => CloseTab(tabs.IndexOf(tab)))
        {
            text = "X"
        };
        closeButton.style.width = 20;
        closeButton.style.marginLeft = 2;
        container.Add(closeButton);

        return container;
    }

    public void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        var closingTab = tabs[index];
        closingTab.DeleteUI();

        if (tabContainers.TryGetValue(closingTab, out var container))
        {
            design.GetSection("tab-section").Remove(container);
            tabContainers.Remove(closingTab);
        }

        bool wasActiveTabClosed = (indexTab == index);
        tabs.RemoveAt(index);

        // Сохраняем обновлённый список вкладок
        window.GetSetting().SetTabs(tabs);

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

    private void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        // Удаляем UI предыдущей активной вкладки
        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].DeleteUI();
        }
        tabs[index].LoadUI();
        tabs[index].OnUI();
        indexTab = index;
        // Сохраняем индекс активной вкладки
        window.GetSetting().SetLastActiveTabIndex(index);
    }

    private void ShowEmptyStateUI()
    {
        VisualElement middleSection = design.GetSection("middle-section-content");
        middleSection.Clear();

        Label emoji = new Label("=[");
        Label message = new Label("It seems you've closed all tabs... Please click the button below to create a new tab.");
        middleSection.Add(emoji);
        middleSection.Add(message);

        Button createNewTabButton = new Button(() =>
        {
            AddTab(new NewTab(middleSection));
        })
        {
            text = "Create New Tab"
        };
        middleSection.Add(createNewTabButton);
    }

    private void ClearEmptyStateUI()
    {
        VisualElement middleSection = design.GetSection("middle-section-content");
        middleSection.Clear();
    }

    /// <summary>
    /// Сохраняет текущий список открытых вкладок в настройках.
    /// Вызывается, например, при закрытии окна.
    /// </summary>
    public void SaveTabsState()
    {
        window.GetSetting().SetTabs(tabs);
    }
    
    public List<EdixorTab> GetTabs()
    {
        return tabs;
    }

    public EdixorDesign GetDesign()
    {
        return design;
    }
}
