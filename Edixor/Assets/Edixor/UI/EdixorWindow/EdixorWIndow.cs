using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EdixorWindow : EditorWindow
{
    private Button toggleButton;
    private ScrollView scrollView;

    public static EdixorWindow currentWindow;

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        currentWindow = GetWindow<EdixorWindow>("EdixorWindow");
        currentWindow.minSize = new Vector2(300, 200);
    }

    private void OnEnable()
    {
        LoadUI();
    }

    private void LoadUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Edixor/UI/EdixorWindow/EdixorWIndow.uxml");
        VisualElement root = visualTree.Instantiate();

        root.style.height = new StyleLength(Length.Percent(100));

        rootVisualElement.Clear();
        rootVisualElement.Add(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Edixor/UI/EdixorWindow/EdixorWIndow.uss");
        root.styleSheets.Add(styleSheet);

        scrollView = root.Q<ScrollView>("scrollView");

    }

    public void RestartWindow()
    {
        if (currentWindow != null && currentWindow == this)
        {
            Debug.Log("Restarting window...");
            Close();
            ShowWindow();
        }
        else
        {
            Debug.Log("Window is not open, skipping restart.");
        }
    }

    private void OnDisable()
    {
        currentWindow = null;
    }
}
