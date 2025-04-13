using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class HotKeyTab : EdixorTab
{
    private HotKeyService hotKeySetting;
    // Список записей, каждая из которых содержит ключ и массив горячих клавиш (данных)
    private List<HotKeySaveAsset.KeyActionDictionaryEntry> hotkeyEntries;
    private VisualElement designContainer;

    // Храним данные, полученные из события (при необходимости можно расширять)
    private string receivedTitle;

    [MenuItem("Window/Edixor Tab/Hot Keys")]
    public static void ShowTab()
    {
        ShowTab<HotKeyTab>();
    }

    private void Awake()
    {
        tabName = "Hot Keys";
        LoadUxml("Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uxml");
        LoadUss("Assets/Edixor/Scripts/UI/EdixorTab/HotKeyTab/HotKeyTab.uss");

        // Подписываемся на событие добавления горячей клавиши
        OnHotKeyAdded += TabProcessing;
    }

    public void OnEnable()
    {
        Debug.Log("HotKeysTab OnEnable called.");
        RefreshHotKeysUI();
    }

    public void OnDisable()
    {
        Debug.Log("HotKeysTab OnDisable called.");
        OnHotKeyAdded -= TabProcessing;
    }

    protected void Start()
    {
        hotKeySetting = container.ResolveNamed<HotKeyService>(ServiceNames.HotKeySetting);
        hotkeyEntries = hotKeySetting.GetAllEntries();

        designContainer = root.Q<VisualElement>("hotkeys-container");
        RefreshHotKeysUI();
    }

    /// <summary>
    /// Обработчик события добавления новой горячей клавиши.
    /// При получении события обновляется UI.
    /// </summary>
    private void TabProcessing(HotKeyTabInfo info)
    {
        receivedTitle = info.TabName;
        Debug.Log("Получено событие добавления горячей клавиши для вкладки: " + info.TabName);
        RefreshHotKeysUI();
    }

    /// <summary>
    /// Обновляет UI: получает актуальные записи из настроек и заново отрисовывает все элементы.
    /// Для каждого элемента, получаемого из настроек, заголовок – это Key,
    /// а под заголовком отрисовывается список горячих клавиш (данных) по данному ключу.
    /// </summary>
    private void RefreshHotKeysUI()
    {
        // Обновляем данные из сервиса
        hotkeyEntries = hotKeySetting.GetAllEntries();

        // Очищаем контейнер, чтобы заново построить список
        if (designContainer != null)
        {
            designContainer.Clear();

            // Обходим каждую запись
            foreach (var entry in hotkeyEntries)
            {
                // Создаём контейнер для группы, соответствующей данному ключу
                VisualElement groupContainer = new VisualElement();
                groupContainer.AddToClassList("hotkey-group");

                // Заголовок группы: используем значение Key
                Label groupHeader = new Label(entry.Key);
                groupHeader.AddToClassList("title");
                groupContainer.Add(groupHeader);

                // Если в записи имеются данные, проходим по массиву Values
                if (entry.Values != null)
                {
                    for (int i = 0; i < entry.Values.Length; i++)
                    {
                        // Добавляем UI-элемент для отдельной горячей клавиши внутри группы
                        groupContainer.Add(CreateHotKeyItem(entry, i));
                    }
                }

                designContainer.Add(groupContainer);
            }
        }
    }

    private VisualElement CreateHotKeyItem(HotKeySaveAsset.KeyActionDictionaryEntry entry, int dataIndex)
    {
        // Контейнер для строки элемента (используем класс hotkeys-box из USS)
        VisualElement hotkeysBox = new VisualElement();
        hotkeysBox.AddToClassList("hotkeys-box");

        // Отображаем заголовок элемента (например, порядковый номер или другой идентификатор)
        Label itemHeader = new Label($"Item {dataIndex + 1}");
        itemHeader.AddToClassList("hotkey-item-header");
        hotkeysBox.Add(itemHeader);

        // Отображаем содержимое (описание или комбинацию) горячей клавиши
        string description = entry.Values[dataIndex].ToString();
        Label itemContent = new Label(description);
        itemContent.AddToClassList("hotkey-item-content");
        hotkeysBox.Add(itemContent);

        // Кнопка "Edit" — выравнивается в той же строке и имеет высоту 40 пикселей
        Button editButton = new Button(() => EditHotKey(entry, dataIndex))
        {
            text = "Edit"
        };
        editButton.AddToClassList("edit-button");
        hotkeysBox.Add(editButton);

        return hotkeysBox;
    }

    /// <summary>
    /// Метод для редактирования конкретной горячей клавиши.
    /// Здесь реализуется логика открытия панели редактирования или модального окна.
    /// </summary>
    /// <param name="entry">Запись горячей клавиши.</param>
    /// <param name="dataIndex">Индекс элемента, который требуется отредактировать.</param>
    private void EditHotKey(HotKeySaveAsset.KeyActionDictionaryEntry entry, int dataIndex)
    {
        Debug.Log($"Редактирование горячей клавиши для ключа: {entry.Key}, индекс: {dataIndex}");
        // Здесь можно открыть окно или форму для изменения данных конкретной горячей клавиши
    }
}
