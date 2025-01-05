using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UxmlMirroringEditor : EditorWindow
{
    private VisualElement rootElement;

    [MenuItem("Window/UXML Mirroring Editor")]
    public static void ShowWindow()
    {
        UxmlMirroringEditor window = GetWindow<UxmlMirroringEditor>();
        window.titleContent = new GUIContent("UXML Mirroring Editor");
    }

    private void OnEnable()
    {
        // Load UXML and add to root.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Test/Custom/Customuxml.uxml");
        var uxmlRoot = visualTree.CloneTree();
        rootElement = uxmlRoot.Q<VisualElement>("root");

        if (rootElement != null)
        {
            // Set size and background for visibility
            rootElement.style.width = Length.Percent(100);
            rootElement.style.height = Length.Percent(100);
            rootElement.style.backgroundColor = new StyleColor(Color.gray);

            uxmlRoot.Add(rootElement);
            rootVisualElement.Add(uxmlRoot);

            // Create buttons for mirroring options.
            CreateMirroringButtons();
        }
        else
        {
            Debug.LogError("The UXML file does not contain a VisualElement with the name 'root'.");
        }
    }

    private void CreateMirroringButtons()
    {
        // Buttons container
        var buttonContainer = new VisualElement();
        buttonContainer.style.flexDirection = FlexDirection.Row;
        buttonContainer.style.justifyContent = Justify.SpaceBetween;
        buttonContainer.style.marginBottom = 10;

        // Standard view button
        var standardButton = new Button(() => ApplyTransform(1, 1)) { text = "Standard" };
        buttonContainer.Add(standardButton);

        // Vertical mirror button
        var verticalButton = new Button(() => ApplyTransform(1, -1)) { text = "Vertical Mirror" };
        buttonContainer.Add(verticalButton);

        // Horizontal mirror button
        var horizontalButton = new Button(() => ApplyTransform(-1, 1)) { text = "Horizontal Mirror" };
        buttonContainer.Add(horizontalButton);

        // Diagonal mirror button
        var diagonalButton = new Button(() => ApplyTransform(-1, -1)) { text = "Diagonal Mirror" };
        buttonContainer.Add(diagonalButton);

        rootVisualElement.Add(buttonContainer);
    }

    private void ApplyTransform(float scaleX, float scaleY)
    {
        if (rootElement == null) return;

        // Apply scaling to the "root" element
        rootElement.style.scale = new Scale(new Vector2(scaleX, scaleY));
    }
}
