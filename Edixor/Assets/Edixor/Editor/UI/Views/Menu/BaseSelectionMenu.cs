using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public abstract class BaseSelectionMenu : EditorWindow
{
    protected List<ICMItem> menuItems = new List<ICMItem>();
    protected Vector2 popupPosition;
    protected float itemHeight = 20f;
    protected StyleParameters styleParameters;

    public static BaseSelectionMenu activeMenu;

    public void AddItem(ICMItem item)
    {
        if (item != null)
            menuItems.Add(item);
    }

    public void AddStyle(StyleParameters styleParameters) {
        if(styleParameters != null)
            this.styleParameters = styleParameters;
    }

    public void ShowMenu(Vector2? position = null)
    {
        if (activeMenu != null)
        {
            BaseSelectionMenu menuToClose = activeMenu;
            activeMenu = null;
            menuToClose.Close();
        }

        activeMenu = this;
        popupPosition = position.HasValue 
            ? position.Value 
            : GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

        float height = menuItems.Count * itemHeight + 3f;
        float width = 200;
        Vector2 windowSize = new Vector2(width, height);

        Rect anchorRect = new Rect(popupPosition.x, popupPosition.y, 0, 0);
        ShowAsDropDown(anchorRect, windowSize);

        CreateUI();
    }

    protected virtual void CreateUI()
    {
        VisualElement root = rootVisualElement;
        root.Clear(); 
        root.AddToClassList("menu-root");
        root.name = "menu-root";

        foreach (ICMItem item in menuItems)
        {
            VisualElement element = item.Draw(); 
            if (element != null)
                root.Add(element);
        }
    }

    protected void CloseMenu()
    {
        Close();
        activeMenu = null;
    }

    protected virtual void OnLostFocus()
    {
        CloseMenu();
    }

    protected virtual void OnDestroy()
    {
        foreach (var menuItem in menuItems)
            menuItem.SaveState();

        if (activeMenu == this)
            activeMenu = null;
    }
}
