using UnityEditor;
using UnityEngine;

public class CustomMenu : Menu<CustomMenu, ICMItem>
{
    // Инициализация стиля меню
    public void Initialize()
    {
        //this.style = style;

        // Загрузка состояния каждого элемента при инициализации
        foreach (var menuItem in menuItems)
        {
            menuItem.LoadState();
        }
    }

    private void OnGUI()
    {
        BeginGUIMenu();  // Начальная обработка GUI

        foreach (var menuItem in menuItems)
        {
            // Отрисовка каждого элемента
            menuItem.Draw(this, itemHeight, new GUIStyle(GUI.skin.button));
        }

        // Закрытие меню при щелчке мыши вне его границ
        if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
        {
            CloseMenu();
        }
    }

    // Закрытие меню при потере фокуса
    private void OnLostFocus()
    {
        CloseMenu();
    }

    // Уничтожение меню
    private void OnDestroy()
    {
        // Сохранение состояния каждого элемента при уничтожении меню
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
