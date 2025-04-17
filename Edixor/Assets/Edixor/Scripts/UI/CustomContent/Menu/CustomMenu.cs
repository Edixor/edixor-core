using UnityEditor;
using UnityEngine;

public class CustomMenu : BaseSelectionMenu
{
    [MenuItem("Window/Custom Menu Example")]
    public static void ShowExampleMenu()
    {
        CustomMenu menu = CreateInstance<CustomMenu>();

        menu.AddItem(new CMItemAction("Action 1", true, () => Debug.Log("Action 1 executed"), "action1"));
        menu.AddItem(new CMItemAction("Action 2", true, () => Debug.Log("Action 2 executed"), "action2"));

        menu.ShowMenu();
    }
}
