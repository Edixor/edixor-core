using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EdixorWindowTest : EditorWindow
{
    private Button toggleButton;
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

        scrollView = root.Q<ScrollView>("scrollView");

        // Добавляем элементы в ScrollView
        for (int i = 0; i < 20; i++)
        {
            scrollView.Add(new Label($"Item {i + 1}"));
        }

        // Применяем обработчик для кнопки "Restart"
        root.Q<Button>("Restart").clicked += RestartWindow;
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
