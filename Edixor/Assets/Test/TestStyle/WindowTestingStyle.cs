using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class WindowTestingStyle : EditorWindow
{
    [SerializeField]
    private string[] style = new string[] {
        "Assets/Test/TestStyle/StyleUSS/Style1.uss",
        "Assets/Test/TestStyle/StyleUSS/Style2.uss",
        "Assets/Test/TestStyle/StyleUSS/Style3.uss"
    };

    private VisualTreeAsset m_VisualTreeAsset = default;
    private VisualElement uxmlContent;
    private StyleSheet currentStyleSheet;

    [MenuItem("Window/UI Toolkit/WindowTestingStyle")]
    public static void ShowExample()
    {
        WindowTestingStyle wnd = GetWindow<WindowTestingStyle>();
        wnd.titleContent = new GUIContent("WindowTestingStyle");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Загрузка и добавление UXML
        string uxmlPath = "Assets/Test/TestStyle/WindowTestingStyle.uxml"; // Укажите реальный путь к вашему UXML
        m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

        if (m_VisualTreeAsset == null)
        {
            Debug.LogError($"Failed to load UXML at path: {uxmlPath}");
            return;
        }

        uxmlContent = m_VisualTreeAsset.Instantiate();
        root.Add(uxmlContent);

        // Добавляем кнопки для выбора стиля
        for (int i = 0; i < style.Length; i++)
        {
            int index = i; // Локальная копия индекса для использования в замыкании
            Button button = new Button(() => ApplyStyle(index))
            {
                text = $"Apply Style {i + 1}"
            };
            root.Add(button);
        }
    }

    private void ApplyStyle(int index)
    {
        if (index < 0 || index >= style.Length)
        {
            Debug.LogError($"Invalid style index: {index}");
            return;
        }

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(style[index]);
        if (styleSheet != null)
        {
            // Проверяем, отличается ли новый стиль от текущего
            if (currentStyleSheet != styleSheet)
            {
                if (currentStyleSheet != null)
                {
                    rootVisualElement.styleSheets.Remove(currentStyleSheet);
                }
                rootVisualElement.styleSheets.Add(styleSheet); // Применяем новый стиль
                currentStyleSheet = styleSheet; // Обновляем текущий стиль
                Debug.Log($"Applied Style {index + 1}: {style[index]}");
            }
        }
        else
        {
            Debug.LogError($"StyleSheet not found at path: {style[index]}");
        }
    }
}
