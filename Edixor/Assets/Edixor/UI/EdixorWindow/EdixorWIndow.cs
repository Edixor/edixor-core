using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class EdixorWindow : EditorWindow
{
    public static EdixorWindow CurrentWindow { get; private set; }

    private VisualElement scrollView;
    private EdixorUIManager uiManager;

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        CurrentWindow = GetWindow<EdixorWindow>("EdixorWindow");
    }

    private void OnEnable()
    {
        uiManager = new EdixorUIManager(this);
        uiManager.LoadUI();
    }

    public void RestartWindow()
    {
        if (CurrentWindow != null)
        {
            Debug.Log("Restarting window...");
            Close();
            ShowWindow();
        }
        else
        {
            Debug.LogWarning("Window is not open, skipping restart.");
        }
    }

    public EdixorUIManager GetUIManager() {
        return uiManager;
    }

    private void OnDisable()
    {
        CurrentWindow = null;
    }
}