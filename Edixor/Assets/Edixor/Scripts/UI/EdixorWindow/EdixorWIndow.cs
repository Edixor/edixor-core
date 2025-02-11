using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class EdixorWindow : EditorWindow
{
    public static EdixorWindow CurrentWindow { get; private set; }
    private VisualElement scrollView;
    private EdixorUIManager uiManager;
    private EdixorWindowSetting setting;
    private EdixorHotKeys hotKeys;

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        CurrentWindow = GetWindow<EdixorWindow>("EdixorWindow");
    }

    private void OnEnable()
    {
        AssetChangesListener.OnRestartPending += RestartWindow;
        InitializeSettings();
        InitializeUI();
        InitializeHotKeys();
    }

    private void OnGUI()
    {
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
        if (CurrentWindow == null)
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

    public EdixorUIManager GetUIManager() => uiManager;
    public EdixorWindowSetting GetSetting() => setting;
    public void UpdateUI() => uiManager?.LoadUI();

    private void OnDisable()
    {
        if (CurrentWindow != this) return;
        try { SaveSettings(); }
        catch (System.Exception e) { Debug.LogError("Failed to save settings: " + e.Message); }
        CurrentWindow = null;
    }

    private void SaveSettings()
    {
        setting?.Save();
    }
}
