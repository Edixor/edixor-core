using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class EdixorWindow : EditorWindow
{
    public static EdixorWindow CurrentWindow { get; private set; }
    private EdixorUIManager uiManager;
    private EdixorWindowSetting setting;
    private EdixorHotKeys hotKeys;

    // Обработчик горячих клавиш
    private HotkeyCaptureHandler hotkeyCaptureHandler = new HotkeyCaptureHandler();

    // Исходный Rect окна перед минимизацией
    private Rect originalWindowRect;

    // Порог минимального размера окна
    private readonly Vector2 minimalSizeThreshold = new Vector2(150, 60);

    // Флаг, указывающий, что окно уже было минимизировано (и исходный Rect сохранён)
    private bool isMinimized = false;

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        CurrentWindow = GetWindow<EdixorWindow>("EdixorWindow");
    }

    private void OnEnable()
    {
        CurrentWindow = this;

        InitializeSettings();

        // Если исходный Rect не задан, сохраняем текущий размер окна как исходный.
        if (originalWindowRect.width == 0 || originalWindowRect.height == 0)
        {
            originalWindowRect = position;
        }

        InitializeUI();
        InitializeHotKeys();

        // Если окно открывается и его размер уже минимален, показываем минимизированный UI.
        if (position.width <= minimalSizeThreshold.x && position.height <= minimalSizeThreshold.y)
        {
            isMinimized = true;
            uiManager?.ShowMinimizedUI();
        }
    }

    private void OnGUI()
    {
        // Обработка горячей клавиши (если происходит)
        if (hotkeyCaptureHandler.IsCapturing())
        {
            hotkeyCaptureHandler.Process(Event.current);
            EditorGUILayout.LabelField("Capturing Hotkey: " + hotkeyCaptureHandler.GetCurrentCombinationString());
            return;
        }

        // Если окно стало минимальным (вручную или программно) и ещё не помечено как минимизированное,
        // сохраняем исходный Rect и отображаем минимизированный UI.
        if (!isMinimized && position.width <= minimalSizeThreshold.x && position.height <= minimalSizeThreshold.y)
        {
            isMinimized = true;
            originalWindowRect = setting.GetOriginalWindowRect().width > 0 
                                    ? setting.GetOriginalWindowRect() 
                                    : position;
            setting.SetOriginalWindowRect(originalWindowRect);
            uiManager?.SaveTabsState();
            uiManager?.ShowMinimizedUI();
        }

        // Остальная логика обработки горячих клавиш
        if (hotKeys == null)
            InitializeHotKeys();
        hotKeys.OnKeys();
    }

    private void InitializeSettings()
    {
        if (setting == null)
        {
            setting = new EdixorWindowSetting(this);
        }
        setting.Load();

        // Передаём окно в каждое горячее действие
        if (setting.GetHotKeys() != null)
        {
            foreach (var action in setting.GetHotKeys())
            {
                action.SetWindow(this);
            }
        }

        setting.SetWindowOpen(true);
    }

    private void InitializeUI()
    {
        uiManager = new EdixorUIManager(this);
        uiManager.LoadUI();
    }

    private void InitializeHotKeys()
    {
        hotKeys = new EdixorHotKeys(this);
    }

    public void RestartWindow()
    {
        if (!setting.IsWindowOpen())
        {
            Debug.LogWarning("Window is not open, skipping restart.");
            return;
        }
        Debug.Log("Restarting window...");
        EditorApplication.delayCall += () =>
        {
            Close();
            ShowWindow();
        };
    }
    
    /// <summary>
    /// Программно минимизирует окно до минимального размера.
    /// Сохраняется исходный Rect перед изменением, после чего окно становится минимальным.
    /// </summary>
    public void MinimizeWindow()
    {
        // Сохраняем текущий (исходный) Rect
        originalWindowRect = position;
        setting.SetOriginalWindowRect(originalWindowRect);

        isMinimized = true;

        // Устанавливаем минимальный размер
        minSize = minimalSizeThreshold;
        position = new Rect(position.x, position.y, minimalSizeThreshold.x, minimalSizeThreshold.y);
        Debug.Log("Window minimized to minimal size: " + minimalSizeThreshold);
        
        uiManager?.SaveTabsState();
        uiManager?.ShowMinimizedUI();
    }
    
    /// <summary>
    /// Возвращает окно к Rect, который был сохранён перед минимизацией.
    /// После восстановления флаг минимизации сбрасывается.
    /// </summary>
    public void ReturnWindowToOriginalSize()
    {
        position = originalWindowRect;
        Debug.Log("Window returned to rect: " + originalWindowRect);
        setting.SetOriginalWindowRect(originalWindowRect);

        isMinimized = false;

        rootVisualElement.Clear();
        InitializeUI();
    }

    /// <summary>
    /// Разворачивает окно на весь экран (максимизация).
    /// </summary>
    public void ExpandWindowToFullScreen()
    {
        this.maximized = true;
        Debug.Log("Window expanded to full screen.");
    }

    public EdixorWindowSetting GetSetting() => setting;
    public EdixorUIManager GetUIManager() => uiManager;
    public EdixorHotKeys GetHotKey() => hotKeys;
    public void UpdateUI() => uiManager?.LoadUI();

    private void OnDisable()
    {
        if (CurrentWindow != this) return;
        try 
        { 
            uiManager?.SaveTabsState();
            SaveSettings();
        }
        catch (System.Exception e) 
        { 
            Debug.LogError("Failed to save settings: " + e.Message); 
        }
        
        setting.SetWindowOpen(false);
        CurrentWindow = null;
    }

    private void SaveSettings()
    {
        setting?.Save();
    }

    /// <summary>
    /// Вызывается для начала захвата новой комбинации клавиш.
    /// Делегирует захват специализированному обработчику.
    /// </summary>
    public void StartHotkeyCapture(System.Action<List<KeyCode>> onHotkeyCaptured)
    {
        hotkeyCaptureHandler.StartCapture(onHotkeyCaptured);
    }
}
