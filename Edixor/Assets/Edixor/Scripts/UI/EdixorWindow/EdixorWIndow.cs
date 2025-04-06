using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

public class EdixorWindow : EditorWindow, IMinimizable, IRestartable, IClosable
{
    public static EdixorWindow CurrentWindow { get; private set; }
    protected DIContainer container;
    protected Rect originalWindowRect;
    protected readonly Vector2 minimalSizeThreshold = new Vector2(150, 60);
    private WindowStateService _windowStateService;
    protected WindowStateService WindowStateService
    {
        get
        {
            if (_windowStateService == null)
            {
                _windowStateService = container.ResolveNamed<WindowStateService>(ServiceNames.WindowStateSetting);
                _windowStateService.SetRootElement(rootVisualElement);
            }
            return _windowStateService;
        }
    }

    private EdixorUIManager _uiManager;
    protected EdixorUIManager UIManager
    {
        get
        {
            if (_uiManager == null)
            {
                WindowStateService.SetRootElement(rootVisualElement);
                _uiManager = container.ResolveNamed<EdixorUIManager>(ServiceNames.EdixorUIManager_EdixorWindow);
            }
            return _uiManager;
        }
    }

    private EdixorHotKeys _hotKeys;
    protected EdixorHotKeys HotKeys
    {
        get
        {
            if (_hotKeys == null)
            {
                _hotKeys = container.ResolveNamed<EdixorHotKeys>(ServiceNames.EdixorHotKeys_EdixorWindow);
            }
            return _hotKeys;
        }
    }

    [MenuItem("Window/EdixorWindow")]
    public static void ShowWindow()
    {
        var container = LoadOrCreateContainer();
        var window = GetWindow<EdixorWindow>("EdixorWindow");
        window.Initialize(container);
    }

    private static DIContainer LoadOrCreateContainer()
    {
        string containerPath = PathResolver.ResolvePath("Assets/Edixor/DIContainer.asset");
        var container = AssetDatabase.LoadAssetAtPath<DIContainer>(containerPath);

        if (container == null)
        {
            container = ScriptableObject.CreateInstance<DIContainer>();
            AssetDatabase.CreateAsset(container, containerPath);
            AssetDatabase.SaveAssets();
        }

        return container;
    }

    public void Initialize(DIContainer container)
    {
        this.container = container;

        container.RegisterNamed<IClosable>(ServiceNames.IClosable_EdixorWindow, this);
        container.RegisterNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow, this);
        container.RegisterNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow, this);

        CurrentWindow = this;

        if (originalWindowRect.width == 0 || originalWindowRect.height == 0)
        {
            originalWindowRect = position;
        }

        InitializeUI();
        InitializeHotKeys();
        SubscribeToEvents();

        // Если окно сразу минимизировано по размерам, выставляем соответствующее состояние
        if (position.width <= minimalSizeThreshold.x && position.height <= minimalSizeThreshold.y)
        {
            WindowStateService.SetMinimized(true);
            //UIManager?.ShowMinimizedUI();
        }
        else
        {
            WindowStateService.SetWindowOpen(true);
        }
    } 

    /// <summary>
    /// Подписка на события UIElements.
    /// </summary>
    private void SubscribeToEvents()
    {
        // Событие, отслеживающее изменение размеров окна
        rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            // Если размеры окна меньше пороговых значений и окно не в состоянии "минимизировано"
            if (!WindowStateService.GetMinimized() &&
                (position.width <= minimalSizeThreshold.x || position.height <= minimalSizeThreshold.y))
            {
                MinimizeWindow();
            }
            // Если окно было минимизировано, а теперь его размер увеличился
            else if (WindowStateService.GetMinimized() &&
                     (position.width > minimalSizeThreshold.x && position.height > minimalSizeThreshold.y))
            {
                ReturnWindowToOriginalSize();
            }
        });

        // Событие для обработки нажатий клавиш
        rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (HotKeys != null && HotKeys.IsHotKeyEnabled())
            {
                HotKeys.OnKeys();
            }
        });
    }

    protected void InitializeUI()
    {
        UIManager.LoadUI();
    }

    protected void InitializeHotKeys()
    {
        HotKeys.InitHotKeys();
    }

    public void RestartWindow()
    {
        if (!WindowStateService.IsWindowOpen())
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

    public void MinimizeWindow()
    {
        // Сохраняем оригинальные размеры, если они еще не сохранены
        if (position.width > minimalSizeThreshold.x && position.height > minimalSizeThreshold.y)
        {
            originalWindowRect = position;
            WindowStateService.SetOriginalWindowRect(originalWindowRect);
        }

        WindowStateService.SetMinimized(true);
        minSize = minimalSizeThreshold;
        position = new Rect(position.x, position.y, minimalSizeThreshold.x, minimalSizeThreshold.y);
        Debug.Log("Window minimized to minimal size: " + minimalSizeThreshold);

        //UIManager?.SaveTabsState();
        //UIManager?.ShowMinimizedUI();
    }

    public void ReturnWindowToOriginalSize()
    {
        position = originalWindowRect;
        Debug.Log("Window returned to rect: " + originalWindowRect);
        WindowStateService.SetOriginalWindowRect(originalWindowRect);
        WindowStateService.SetMinimized(false);
        rootVisualElement.Clear();
        InitializeUI();
    }

    public void CloseWindow()
    {
        Close();
    }

    protected virtual void OnDisable()
    {
        if (CurrentWindow != this) return;

        try 
        { 
            //UIManager?.SaveTabsState();
            SaveSettings();
        }
        catch (System.Exception e) 
        { 
            Debug.LogError("Failed to save settings: " + e.Message); 
        }
        
        WindowStateService.SetWindowOpen(false);
        CurrentWindow = null;
    }

    protected void SaveSettings()
    {
        WindowStateService?.Save();
    }
}
