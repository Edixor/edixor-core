using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using System;

public class EdixorWindow : EditorWindow, IMinimizable, IRestartable, IClosable
{
    public static EdixorWindow CurrentWindow { get; private set; }
    protected DIContainer container;
    protected Rect originalWindowRect;
    protected readonly Vector2 minimalSizeThreshold = new Vector2(150, 60);
    private bool _initialized = false;

    private WindowStateService _windowStateService;
    private HotKeyController _hotKeys;
    private ITabController _tabService;

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

    private IEdixorInterfaceFacade _setting;

    protected IEdixorInterfaceFacade setting 
    {
        get
        {
            if (_setting == null)
            {
                _setting = container.ResolveNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorWindow);
            }
            return _setting;
        }
    }

    public static void ShowWindow<T>(string title) where T : EdixorWindow
    {
        var window = GetWindow<T>(title);
        window.Show();
    }

    protected virtual void OnEnable()
    {
        if (_initialized) return;
        
        container = LoadOrCreateContainer();
        Initialize();
        Options();
        _initialized = true;
    }

    protected virtual void Options() {
        
    }

    protected static DIContainer LoadOrCreateContainer()
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

    private void Initialize()
    {
        container.RegisterNamed<IClosable>(ServiceNames.IClosable_EdixorWindow, this);
        container.RegisterNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow, this);
        container.RegisterNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow, this);

        CurrentWindow = this;

        if (originalWindowRect.width == 0 || originalWindowRect.height == 0)
        {
            originalWindowRect = position;
        }
        setting.InitOptions(rootVisualElement, container);
        setting.Initialize();
        SubscribeToEvents();

        if (position.width <= minimalSizeThreshold.x && position.height <= minimalSizeThreshold.y)
        {
            WindowStateService.SetMinimized(true);
        }
        else
        {
            WindowStateService.SetWindowOpen(true);
        }

        FileDragHandler.RegisterDragHandlers(rootVisualElement, OnTabAddedFromDrag);
    }

    private void OnTabAddedFromDrag(string filePath, string className)
    {
        Debug.Log($"Tab {className} is being added from file: {filePath}");

        FileDragHandler.AddTabFromFile(filePath, className, (file, tabType) =>
        {
            EdixorTab tab = (EdixorTab)Activator.CreateInstance(tabType);
            setting.AddTab(tab, saveState: false, autoSwitch: true);
        });
    }

    private void SubscribeToEvents()
    {
        rootVisualElement.focusable = true;
        rootVisualElement.pickingMode = PickingMode.Position;

        rootVisualElement.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            if (!WindowStateService.GetMinimized() &&
                (position.width <= minimalSizeThreshold.x || position.height <= minimalSizeThreshold.y))
            {
                MinimizeWindow();
            }
            else if (WindowStateService.GetMinimized() &&
                     (position.width > minimalSizeThreshold.x && position.height > minimalSizeThreshold.y))
            {
                ReturnWindowToOriginalSize();
            }
        });

        rootVisualElement.RegisterCallback<KeyDownEvent>(evt =>
        {
            setting.OnKeys();
        });

        rootVisualElement.Focus();
    }

    private void OnGUI()
    {
        if (container != null)
        {
            if (WindowStateService.GetMinimized())
            {
                return;
            }

            setting.Update();
        }
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
            ShowWindow<EdixorWindow>("EdixorWindow");
        };
    }

    public void MinimizeWindow()
    {
        if (position.width > minimalSizeThreshold.x && position.height > minimalSizeThreshold.y)
        {
            originalWindowRect = position;
            WindowStateService.SetOriginalWindowRect(originalWindowRect);
        }

        WindowStateService.SetMinimized(true);
        minSize = minimalSizeThreshold;
        position = new Rect(position.x, position.y, minimalSizeThreshold.x, minimalSizeThreshold.y);
        Debug.Log("Window minimized to minimal size: " + minimalSizeThreshold);
    }

    public void ReturnWindowToOriginalSize()
    {
        position = originalWindowRect;
        WindowStateService.SetOriginalWindowRect(originalWindowRect);
        WindowStateService.SetMinimized(false);
        rootVisualElement.Clear();
        setting.Initialize();
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
            SaveSettings();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }

        setting.OnWindowClose();
        WindowStateService.SetWindowOpen(false);
        CurrentWindow = null;
    }

    protected void SaveSettings()
    {
        WindowStateService?.Save();
    }
}
