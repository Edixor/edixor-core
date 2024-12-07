using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CustomEnum : Menu<CustomEnum, CMItemBool>
{
    // Инициализация меню с заданным стилем
    public void Initialize(BFPStyle style)
    {
        this.style = style;

        // Загрузка состояний всех элементов при инициализации
        foreach (var menuItem in menuItems)
        {
            menuItem.LoadState();
        }
    }

    // Отрисовка меню
    private void OnGUI()
    {
        BeginGUIMenu();  // Вызов базового метода для начальной обработки GUI

        foreach (var menuItem in menuItems)
        {
            // Полиморфный вызов метода Draw для каждого элемента
            menuItem.Draw(this, itemHeight, new GUIStyle(GUI.skin.button));
        }

        // Закрытие меню при щелчке мыши вне его границ
        if (Event.current.type == EventType.MouseDown && !position.Contains(Event.current.mousePosition))
        {
            CloseMenu();
        }
    }

    // Обработка выбора элемента
    public override void HandleItemSelection(ICMItem selectedItem)
    {
        if (selectedItem is CMItemBool selectedBoolItem)
        {
            foreach (var item in menuItems)
            {
                if (item is CMItemBool boolItem)
                {
                    // Устанавливаем состояние: выбранный элемент - true, остальные - false
                    boolItem.SetSelected(boolItem == selectedBoolItem);

                    // Сохраняем состояние каждого элемента
                    boolItem.SaveState();
                }
            }

            // Выводим в консоль состояние всех элементов
            Debug.Log("Состояние всех элементов:");
            foreach (var item in menuItems)
            {
                if (item is CMItemBool boolItem)
                {
                    Debug.Log($"{boolItem.Name}: {boolItem.IsSelected}");
                }
            }
        }

        // Закрываем меню после выбора элемента
        CloseMenu();
    }

    // Закрытие меню при потере фокуса
    private void OnLostFocus()
    {
        CloseMenu();
    }

    // Уничтожение меню
    private void OnDestroy()
    {
        // Сохраняем состояния всех элементов перед уничтожением меню
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
