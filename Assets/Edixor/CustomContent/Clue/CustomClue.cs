using UnityEditor;
using UnityEngine;

public class CustomClue : EditorWindow
{
    private Rect labelRect;

    public void SetLabelRect(Rect rect)
    {
        labelRect = rect;
    }

    private void OnGUI()
    {
        // Окно Clue будет пустым, но будет выводить сообщение в консоль
        if (labelRect.Contains(Event.current.mousePosition))
        {
            Debug.Log("Cursor is hovering over the label in CustomEditorWindow.");
        }
    }
}
