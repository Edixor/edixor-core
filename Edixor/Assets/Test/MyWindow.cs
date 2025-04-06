using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MyWindow : EdixorWindow
{
    [MenuItem("Window/MyWindow")]
    public static void ShowExample()
    {
        MyWindow wnd = GetWindow<MyWindow>();
        wnd.titleContent = new GUIContent("MyWindow");
    }

    public void CreateGUI()
    {
        Button myButton = new Button(() => OnButtonClick())
        {
            text = "Нажми меня"
        };
        
        rootVisualElement.Add(myButton);
    }

    private void OnButtonClick()
    {
        Debug.Log("Кнопка нажата!");
    }
}
