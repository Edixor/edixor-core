using UnityEngine;

public static class ClueManager
{
    private static CustomClue clueWindow;
    private static bool isClueOpen = false;
    private static bool isClueInitialized = false;

    public static void CheckAndOpenClue(Rect labelRect)
    {
        if (labelRect.Contains(Event.current.mousePosition))
        {
            if (!isClueInitialized || clueWindow == null)
            {
                // Создаём или активируем окно CustomClue
                //clueWindow = CustomEditorWindow.GetOrCreateClueWindow();
                clueWindow.SetLabelRect(labelRect);
                clueWindow.ShowUtility(); // Открытие как Utility окно
                isClueInitialized = true;
                isClueOpen = true;
            }
            else if (!isClueOpen)
            {
                // Показываем окно, если оно было инициализировано, но закрыто
                clueWindow.ShowUtility(); // Открытие как Utility окно
                isClueOpen = true;
            }
        }
        else
        {
            if (isClueOpen && clueWindow != null)
            {
                // Закрываем окно CustomClue и сбрасываем инициализацию
                clueWindow.Close();
                isClueOpen = false;
                isClueInitialized = false;  // Сброс состояния для возможности повторного создания
            }
        }
    }
}
