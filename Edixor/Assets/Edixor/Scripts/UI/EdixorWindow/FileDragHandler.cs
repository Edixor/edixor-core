using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class FileDragHandler
{
    public static void RegisterDragHandlers(VisualElement rootVisualElement, Action<string, string> onTabAddedCallback)
    {
        rootVisualElement.RegisterCallback<DragUpdatedEvent>(evt => OnDragUpdated(evt));
        rootVisualElement.RegisterCallback<DragPerformEvent>(evt => OnDragPerform(evt, onTabAddedCallback));
    }

    private static void OnDragUpdated(DragUpdatedEvent evt)
    {
        // Проверка, что перетаскиваем C# файл
        if (DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].EndsWith(".cs"))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // Устанавливаем режим копирования
            EditorWindow.focusedWindow.Repaint();
        }
    }

    private static void OnDragPerform(DragPerformEvent evt, Action<string, string> onTabAddedCallback)
    {
        // Получаем перетащенные файлы
        string[] draggedFiles = DragAndDrop.paths;

        foreach (string filePath in draggedFiles)
        {
            // Проверяем, является ли файл C# файлом
            if (filePath.EndsWith(".cs"))
            {
                string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
                Debug.Log($"Attempting to add tab from file: {filePath}");

                // Вызываем обратный вызов для добавления вкладки
                onTabAddedCallback(filePath, className);
            }
        }

        DragAndDrop.AcceptDrag();
        evt.StopPropagation();
    }

    public static void AddTabFromFile(string filePath, string className, Action<string, Type> addTabCallback)
    {
        try
        {
            // Загружаем сборку, чтобы найти тип по имени
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // Проверяем, есть ли в сборке нужный тип
                Type tabType = assembly.GetType(className);
                if (tabType != null)
                {
                    Debug.Log($"Found class: {tabType.Name}");
                }

                if (tabType != null && tabType.IsSubclassOf(typeof(EdixorTab)))
                {
                    // Создаем экземпляр класса
                    EdixorTab tab = (EdixorTab)Activator.CreateInstance(tabType);

                    // Вызываем обратный вызов для добавления вкладки через UIManager
                    addTabCallback(filePath, tab.GetType());
                    Debug.Log($"Tab {className} added successfully from file {filePath}.");
                    return;
                }
            }

            Debug.LogError($"Class {className} not found or does not inherit from EdixorTab.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error while adding tab from file {filePath}: {ex.Message}");
        }
    }
}
