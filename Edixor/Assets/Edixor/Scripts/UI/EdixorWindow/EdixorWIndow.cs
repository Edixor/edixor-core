using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorWindow : EditorWindow
{
    public static EdixorWindow CurrentWindow { get; private set; }
    private VisualElement scrollView;
    private EdixorUIManager uiManager;
    private EdixorWindowSetting setting;

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        CurrentWindow = GetWindow<EdixorWindow>("EdixorWindow");
    }

    private void OnEnable()
    {
        InitializeSettings();
        InitializeUI();
    }

    private void InitializeSettings()
    {
        if (setting == null)
        {
            setting = new EdixorWindowSetting(this);
        }

        // Проверяем существование настроек и создаем их, если их нет.
        setting.Load();
    }

    private void InitializeUI()
    {
        uiManager = new EdixorUIManager(this);
        uiManager.LoadUI();
    }

    
    public void RestartWindow()
    {
        if (CurrentWindow == null)
        {
            Debug.LogWarning("Window is not open, skipping restart.");
            return;
        }

        Debug.Log("Restarting window...");
        // Отключаем вызов Close, чтобы избежать повторного разрушения окна
        EditorApplication.delayCall += () =>
        {
            Close(); // Закрываем окно в следующем кадре
            ShowWindow(); // Открываем новое окно
        };
    }


    public EdixorUIManager GetUIManager()
    {
        return uiManager;
    }

    public EdixorWindowSetting GetSetting()
    {
        return setting;
    }

    private void OnDisable()
    {
        if (CurrentWindow != this) return;

        try
        {
            SaveSettings();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }

        CurrentWindow = null;
    }


    private void SaveSettings()
    {
        if (setting != null)
        {
            try
            {
                setting.Save();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save settings: {e.Message}");
            }
        }
    }
}
