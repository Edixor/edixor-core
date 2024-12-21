using UnityEngine;
using UnityEngine.UIElements;
using Commands;

public class TestRealistion : MonoBehaviour
{
    Button startButton;

    public Button myButton;
    public VisualElement root;
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        myButton = root.Q<Button>("createuss");

        myButton.clicked += OnButtonClicked;
    }

    void OnButtonClicked() {
        Debug.Log("кнопка нажалась");
        string[] lines = {
            ".custom-label {",
            "    font-size: 20px;",
            "    -unity-font-style: bold;",
            "    color: rgb(68, 138, 255);",
            "}"
        };
        Command command = new CreateUSSFile("Assets/Editor/darab.uss", "darab.uss", lines);
        Debug.Log("VSE");
        command.Tasks();
    }

    void Update()
    {
        
    }
}
