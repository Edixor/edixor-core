using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class EdixorWindow : EditorWindow
{
    public static EdixorWindow CurrentWindow { get; private set; }
    private VisualElement scrollView;
    private EdixorUIManager uiManager;
    private EdixorWindowSetting setting;
    private EdixorHotKeys hotKeys;

    // Экземпляр обработчика захвата горячей клавиши
    private HotkeyCaptureHandler hotkeyCaptureHandler = new HotkeyCaptureHandler();

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        CurrentWindow = GetWindow<EdixorWindow>("EdixorWindow");
    }

    private void OnEnable()
    {
        CurrentWindow = this;

        InitializeSettings();
        InitializeUI();
        InitializeHotKeys();
    }

    private void OnGUI()
    {
        // Если идёт захват новой комбинации, делегируем обработку HotkeyCaptureHandler'у
        if (hotkeyCaptureHandler.IsCapturing())
        {
            hotkeyCaptureHandler.Process(Event.current);
            // Выводим информацию о текущей комбинации (опционально)
            EditorGUILayout.LabelField("Capturing Hotkey: " + hotkeyCaptureHandler.GetCurrentCombinationString());
            // Не выполняем основную логику горячих клавиш, пока идёт захват
            return;
        }

        // Обычная логика окна и обработка горячих клавиш
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

    public EdixorWindowSetting GetSetting() => setting;
    public EdixorUIManager GetUIManager() => uiManager;
    public EdixorHotKeys GetHotKey() => hotKeys;
    public void UpdateUI() => uiManager?.LoadUI();

    private void OnDisable()
    {
        if (CurrentWindow != this) return;
        try 
        { 
            // Сохраняем список открытых вкладок перед закрытием окна
            uiManager?.SaveTabsState();
            SaveSettings();
        }
        catch (System.Exception e) 
        { 
            Debug.LogError("Failed to save settings: " + e.Message); 
        }
        
        // Обновляем состояние окна – отмечаем, что окно закрывается
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
