using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ExTools;
using System;

public class FileDragHandler
{
    public static void RegisterDragHandlers(VisualElement rootVisualElement, Action<string, string> onTabAddedCallback)
    {
        rootVisualElement.RegisterCallback<DragUpdatedEvent>(evt => OnDragUpdated(evt));
        rootVisualElement.RegisterCallback<DragPerformEvent>(evt => OnDragPerform(evt, onTabAddedCallback));
    }

    private static void OnDragUpdated(DragUpdatedEvent evt)
    {
         
        if (DragAndDrop.paths.Length > 0 && DragAndDrop.paths[0].EndsWith(".cs"))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;  
            EditorWindow.focusedWindow.Repaint();
        }
    }

    private static void OnDragPerform(DragPerformEvent evt, Action<string, string> onTabAddedCallback)
    {
         
        string[] draggedFiles = DragAndDrop.paths;

        foreach (string filePath in draggedFiles)
        {
             
            if (filePath.EndsWith(".cs"))
            {
                string className = System.IO.Path.GetFileNameWithoutExtension(filePath);
                ExDebug.Log($"Attempting to add tab from file: {filePath}");

                 
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
             
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                 
                Type tabType = assembly.GetType(className);
                if (tabType != null)
                {
                    ExDebug.Log($"Found class: {tabType.Name}");
                }

                if (tabType != null && tabType.IsSubclassOf(typeof(TabData)))
                {
                     
                    TabData tab = (TabData)Activator.CreateInstance(tabType);

                     
                    addTabCallback(filePath, tab.GetType());
                    ExDebug.Log($"Tab {className} added successfully from file {filePath}.");
                    return;
                }
            }

            ExDebug.LogError($"Class {className} not found or does not inherit.");
        }
        catch (Exception ex)
        {
            ExDebug.LogError($"Error while adding tab from file {filePath}: {ex.Message}");
        }
    }
}
