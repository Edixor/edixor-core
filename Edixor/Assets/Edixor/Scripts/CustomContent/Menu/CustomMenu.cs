using UnityEditor;
using UnityEngine;

public class CustomMenu : Menu<CustomMenu, ICMItem>
{

    public void Initialize()
    {



        foreach (var menuItem in menuItems)
        {
            menuItem.LoadState();
        }
    }

    private void OnGUI()
    {
        BeginGUIMenu();

        foreach (var menuItem in menuItems)
        {

            menuItem.Draw(this, itemHeight, new GUIStyle(GUI.skin.button));
        }


        if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
        {
            CloseMenu();
        }
    }


    private void OnLostFocus()
    {
        CloseMenu();
    }


    private void OnDestroy()
    {

        foreach (var menuItem in menuItems)
        {
            menuItem.SaveState();
        }

        if (activeMenu == this)
        {
            activeMenu = null;
        }
    }
}
