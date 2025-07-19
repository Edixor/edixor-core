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


                clueWindow.SetLabelRect(labelRect);
                clueWindow.ShowUtility();
                isClueInitialized = true;
                isClueOpen = true;
            }
            else if (!isClueOpen)
            {

                clueWindow.ShowUtility();
                isClueOpen = true;
            }
        }
        else
        {
            if (isClueOpen && clueWindow != null)
            {

                clueWindow.Close();
                isClueOpen = false;
                isClueInitialized = false;
            }
        }
    }
}
