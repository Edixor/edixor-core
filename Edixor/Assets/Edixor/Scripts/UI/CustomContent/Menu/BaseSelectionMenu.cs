using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public abstract class BaseSelectionMenu : EditorWindow
{
    protected List<ICMItem> menuItems = new List<ICMItem>();
    protected Vector2 popupPosition;
    protected float itemHeight = 25f; // высота одного пункта меню

    public static BaseSelectionMenu activeMenu;

    public void AddItem(ICMItem item)
    {
        if (item != null)
            menuItems.Add(item);
    }

    /// <summary>
    /// Открывает меню. Если позиция не передана, используется позиция мыши из Event.current.
    /// Важно: вызов должен осуществляться из OnGUI, чтобы Event.current был доступен.
    /// </summary>
    public void ShowMenu(Vector2? position = null)
    {
        // Если уже существует активное меню – закрываем его
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

        // Вычисляем высоту как сумму высот всех пунктов меню без дополнительного отступа
        float height = menuItems.Count * itemHeight;
        float width = 200;
        Vector2 windowSize = new Vector2(width, height);

        // Создаём Rect с нулевым размером; именно эта точка будет «якорем» окна
        Rect anchorRect = new Rect(popupPosition.x, popupPosition.y, 0, 0);
        ShowAsDropDown(anchorRect, windowSize);
    }

    protected virtual void OnGUI()
    {
        // Рисуем все пункты меню
        foreach (var menuItem in menuItems)
            menuItem.Draw();

        // Если произошёл клик вне области окна меню, закрываем его
        if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
            CloseMenu();
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
