using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorWindowTest : EditorWindow
{
    private List<EdixorFunction> boxFunction1 = new List<EdixorFunction>() {};
    private List<EdixorFunction> boxFunction2 = new List<EdixorFunction>() {};
    private ScrollView scrollView;

    // Статическая переменная для хранения текущего окна
    private static EdixorWindowTest currentWindow;

    [MenuItem("Window/EdixorWindowTest")]
    public static void ShowWindow()
    {
        currentWindow = GetWindow<EdixorWindowTest>("EdixorWindowTest");
        currentWindow.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        LoadUI();
    }

    private void LoadUI()
    {
        // Загружаем UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Test/TestWindowEdixor/tesy.uxml");
        VisualElement root = visualTree.Instantiate();

        root.style.height = new StyleLength(Length.Percent(100));

        rootVisualElement.Clear(); // Очистить старые элементы
        rootVisualElement.Add(root); // Добавляем новые элементы

        // Применяем USS стили
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Test/TestWindowEdixor/tesy.uss");
        root.styleSheets.Add(styleSheet);

        // Добавляем кнопки в секции
        VisualElement leftSection = root.Q<VisualElement>(className: "left-section");
        VisualElement rightSection = root.Q<VisualElement>(className: "right-section");

        if (leftSection != null)
        {
            AddButtonsWithIconsToSection(leftSection, boxFunction1);
        }

        if (rightSection != null)
        {
            AddButtonsWithIconsToSection(rightSection, boxFunction2);
        }
    }

    private void AddButtonsWithIconsToSection(VisualElement section, List<EdixorFunction> functions)
    {
        foreach (var func in functions)
        {
            Button button = new Button(() => func.Activate());
            button.name = func.Name;

            // Добавляем изображение на кнопку
            var icon = new Image { image = func.Icon };
            button.Add(icon);

            section.Add(button);
        }
    }

    // Проверка, если окно открыто, то перезапускать его
    public void RestartWindow()
    {
        // Если окно существует и открыто, то закрыть и перезапустить его
        if (currentWindow != null && currentWindow == this)
        {
            Debug.Log("Restarting window...");
            Close(); // Закрываем текущее окно
            ShowWindow(); // Открываем окно заново
        }
        else
        {
            Debug.Log("Window is not open, skipping restart.");
        }
    }

    // Для отслеживания закрытия окна
    private void OnDisable()
    {
        currentWindow = null;
    }
}
