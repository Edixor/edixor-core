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
    private VisualElement root;

    public EdixorUIManager(EdixorWindow window)
    {
        this.window = window;
        root = window.rootVisualElement;
    }

    public void LoadUI()
    {
        design = window.GetSetting().GetCurrentDesign();
        design.LoadUI();
        
        // Если уже есть открытые вкладки в памяти, не восстанавливаем их заново.
        if (tabs.Count == 0)
        {
            if (!RestoreTabs())
            {
                // Если вкладок не было сохранено – создаём первую вкладку по умолчанию
                AddTab(new NewTab(design.GetSection("middle-section-content")), saveState: false, autoSwitch: false);
            }
        }
        else
        {
            // Если вкладки уже существуют, повторно отрисовываем активную вкладку
            if (indexTab >= 0 && indexTab < tabs.Count)
            {
                tabs[indexTab].LoadUI();
                tabs[indexTab].OnUI();
            }
        }
    }

    public void ShowMinimizedUI()
    {
        // Очищаем текущий UI.
        root.Clear();

        // Создаём контейнер для минимизированного UI.
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;
        container.style.alignItems = Align.Center;
        container.style.justifyContent = Justify.Center;
        container.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
        container.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
        
        // Упрощённое информационное сообщение.
        Label infoLabel = new Label("click return");
        infoLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        container.Add(infoLabel);
        
        // Краткая информация об исходном размере.
        Rect originalRect = window.GetSetting().GetOriginalWindowRect();
        string sizeInfo = $"{originalRect.width} x {originalRect.height}";
        Label sizeLabel = new Label(sizeInfo);
        sizeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
        container.Add(sizeLabel);
        
        // Создаем кнопку "return", которая возвращает окно к исходному размеру.
        Button restoreButton = new Button(() =>
        {
            window.ReturnWindowToOriginalSize();
        })
        {
            text = "return"
        };
        container.Add(restoreButton);
        
        // Добавляем контейнер в корневой элемент UI.
        root.Add(container);
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
            
            // Восстанавливаем каждую вкладку без авто-переключения
            foreach (EdixorTab tab in savedTabs)
            {
                tab.SetWindow(window);
                tab.SetParentContainer(middleSection);
                tab.Init();
                AddTab(tab, saveState: false, autoSwitch: false);
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

    // Перегруженный метод AddTab с параметрами для сохранения состояния и авто-переключения
    public void AddTab(EdixorTab newTab, bool saveState = true, bool autoSwitch = true)
    {
        if (newTab == null) return;

        // Если ранее отображался empty state, очищаем центральную область
        ClearEmptyStateUI();

        tabs.Add(newTab);
        var container = CreateTabContainer(newTab);
        tabContainers[newTab] = container;
        design.GetSection("tab-section").Add(container);
        
        if (saveState)
            window.GetSetting().SetTabs(tabs);

        if (autoSwitch)
            SwitchTab(tabs.IndexOf(newTab));
    }

    // Создание контейнера для вкладки с кнопками переключения и закрытия
    private VisualElement CreateTabContainer(EdixorTab tab)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        container.style.marginRight = 0; // отступ между вкладками

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
        closeButton.style.marginLeft = 0;
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
        if (index < 0 || index >= tabs.Count)
            return;

        // Если есть текущая активная вкладка, деактивируем её и удаляем её UI
        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].Deactivate();
            tabs[indexTab].DeleteUI();
        }
        tabs[index].LoadUI();
        tabs[index].OnUI();
        indexTab = index;
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
