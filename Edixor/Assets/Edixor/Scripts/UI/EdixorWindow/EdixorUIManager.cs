using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorUIManager
{
    private List<EdixorTab> tabs = new List<EdixorTab>();
    private int indexTab = -1;
    // Словарь для хранения соответствия между вкладкой и её контейнером (с кнопками)
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
        
        // Создаем первую вкладку при запуске
        AddTab(new NewTab(design.GetSection("middle-section-content")));
    }

    // Метод создания контейнера для вкладки с кнопкой переключения и кнопкой закрытия
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

    public void AddTab(EdixorTab newTab)
    {
        if (newTab == null) return;

        // Если ранее отображался empty state, очищаем центральную область
        ClearEmptyStateUI();

        tabs.Add(newTab);
        var container = CreateTabContainer(newTab);
        tabContainers[newTab] = container;
        design.GetSection("tab-section").Add(container);
        
        // Переключаемся на вновь добавленную вкладку
        SwitchTab(tabs.IndexOf(newTab));
    }

    // Метод закрытия вкладки по её индексу
    private void CloseTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        var closingTab = tabs[index];
        // Удаляем UI закрываемой вкладки
        closingTab.DeleteUI();

        // Удаляем контейнер вкладки из секции вкладок
        if (tabContainers.TryGetValue(closingTab, out var container))
        {
            design.GetSection("tab-section").Remove(container);
            tabContainers.Remove(closingTab);
        }

        // Определяем, была ли закрыта активная вкладка
        bool wasActiveTabClosed = (indexTab == index);

        // Удаляем вкладку из списка
        tabs.RemoveAt(index);

        // Если больше нет вкладок, показываем empty state
        if (tabs.Count == 0)
        {
            indexTab = -1;
            ShowEmptyStateUI();
            return;
        }

        if (wasActiveTabClosed)
        {
            // Если закрытая вкладка была активной, переключаемся на соседнюю вкладку
            int newActiveIndex = index > 0 ? index - 1 : 0;
            SwitchTab(newActiveIndex);
        }
        else
        {
            // Если закрытая вкладка находилась перед активной, корректируем индекс активной вкладки
            if (indexTab > index)
                indexTab--;

            // Перезагружаем UI активной вкладки, чтобы гарантировать его отображение
            SwitchTab(indexTab);
        }
    }

    // Метод переключения между вкладками
    private void SwitchTab(int index)
    {
        if (index < 0 || index >= tabs.Count) return;

        // Если ранее была активная вкладка, удаляем её UI
        if (indexTab >= 0 && indexTab < tabs.Count)
        {
            tabs[indexTab].DeleteUI();
        }
        tabs[index].LoadUI();
        tabs[index].OnUI();
        indexTab = index;
    }

    // Метод, который отображает пустое состояние, если вкладок нет
    private void ShowEmptyStateUI()
    {
        VisualElement middleSection = design.GetSection("middle-section-content");
        middleSection.Clear();

        // Сообщение для пользователя на английском
        Label message = new Label("It seems you've closed all tabs... Please click the button below to create a new tab.");
        middleSection.Add(message);

        // Кнопка для создания новой вкладки на английском
        Button createNewTabButton = new Button(() =>
        {
            // Создаем новую вкладку, передавая тот же контейнер для контента
            AddTab(new NewTab(middleSection));
        })
        {
            text = "Create New Tab"
        };
        middleSection.Add(createNewTabButton);
    }

    // Метод для очистки пустого состояния, если оно отображалось
    private void ClearEmptyStateUI()
    {
        VisualElement middleSection = design.GetSection("middle-section-content");
        // Предполагается, что при создании новой вкладки UI полностью обновляется
        middleSection.Clear();
    }

    public EdixorDesign GetDesign()
    {
        return design;
    }
}
