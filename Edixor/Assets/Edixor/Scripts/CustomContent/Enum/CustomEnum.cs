using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CustomEnum : Menu<CustomEnum, CMItemBool>
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


    public override void HandleItemSelection(ICMItem selectedItem)
    {
        if (selectedItem is CMItemBool selectedBoolItem)
        {
            foreach (var item in menuItems)
            {
                if (item is CMItemBool boolItem)
                {

                    boolItem.SetSelected(boolItem == selectedBoolItem);


                    boolItem.SaveState();
                }
            }


            Debug.Log("Состояние всех элементов:");
            foreach (var item in menuItems)
            {
                if (item is CMItemBool boolItem)
                {
                    Debug.Log($"{boolItem.Name}: {boolItem.IsSelected}");
                }
            }
        }


        CloseMenu();
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
