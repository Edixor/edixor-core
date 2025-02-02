using UnityEditor;
using UnityEngine;

public class KeyPressWindow : EditorWindow
{
    [MenuItem("Window/KeyPressWindow")]
    public static void ShowWindow()
    {
        var window = GetWindow<KeyPressWindow>();
        window.titleContent = new GUIContent("Key Press Window");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Нажмите Shift + X, чтобы вывести сообщение в консоль.");

        // Обрабатываем события клавиатуры
        Event e = Event.current;
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.X && e.shift)
        {
            Debug.Log("Комбинация Shift + X была нажата!");
            e.Use(); // Указываем, что событие обработано
        }
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.X)
        {
            Debug.Log("Комбинация X была нажата!");
            e.Use(); // Указываем, что событие обработано
        }
    }
}
