using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using ExTools;
using System;

public class EdixorWindow : EditorWindow, IMinimizable, IRestartable, IClosable
{
    private static readonly HashSet<Type> _initializedWindowTypes = new HashSet<Type>();
    private string _title = "None";
    public static EdixorWindow CurrentWindow { get; private set; }
    protected DIContainer container;
    protected Rect originalWindowRect;
    protected readonly Vector2 minimalSizeThreshold = new Vector2(150, 60);

    private WindowStateSetting _windowStateService;
    protected WindowStateSetting WindowStateSetting
    {
        get
        {
            if (_windowStateService == null)
            {
                _windowStateService = container.ResolveNamed<WindowStateSetting>(ServiceNames.WindowStateSetting);
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
                _setting = container.ResolveNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorWindow);
            return _setting;
        }
    }

    public static void ShowWindow<T>(string title) where T : EdixorWindow
    {
        T window = GetWindow<T>(title);
        window.Show();
    }

    protected virtual void OnEnable()
    {
        _title = this.titleContent.text;
        ExDebug.BeginGroup(_title + " is open");

        container = LoadOrCreateContainer();
        RegisterServers();
        setting.InitOptions(rootVisualElement, container);
        if (_initializedWindowTypes.Add(GetType()))
            OnOptions();
        Initialize();
        setting.Initialize();
    }

    protected virtual void OnOptions() { }

    protected static DIContainer LoadOrCreateContainer()
    {
        string path = PathResolver.ResolvePath("Assets/Edixor/Editor/Core/Services/DIContainer.asset");
        var c = AssetDatabase.LoadAssetAtPath<DIContainer>(path);
        if (c == null)
        {
            c = ScriptableObject.CreateInstance<DIContainer>();
            AssetDatabase.CreateAsset(c, path);
            AssetDatabase.SaveAssets();
        }
        return c;
    }

    private void RegisterServers() {
        container.RegisterNamed<IClosable>(ServiceNames.IClosable_EdixorWindow, this);
        container.RegisterNamed<IMinimizable>(ServiceNames.IMinimizable_EdixorWindow, this);
        container.RegisterNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow, this);
    }

    private void Initialize()
    {
        CurrentWindow = this;

        if (originalWindowRect.width == 0 && originalWindowRect.height == 0)
            originalWindowRect = position;

        SubscribeToEvents();

        if (position.width <= minimalSizeThreshold.x && position.height <= minimalSizeThreshold.y)
            WindowStateSetting.SetMinimized(true);
        else
            WindowStateSetting.SetWindowOpen(true);

        FileDragHandler.RegisterDragHandlers(rootVisualElement, OnTabAddedFromDrag);
    }

    private void OnTabAddedFromDrag(string filePath, string className)
    {
        ExDebug.Log($"Tab {className} is being added from file: {filePath}");

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
            if (!WindowStateSetting.GetMinimized() &&
                (position.width <= minimalSizeThreshold.x || position.height <= minimalSizeThreshold.y))
            {
                MinimizeWindow();
            }
            else if (WindowStateSetting.GetMinimized() &&
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
            if (WindowStateSetting.GetMinimized())
            {
                return;
            }

            setting.Update();
        }
    }

    public void RestartWindow()
    {
        if (!WindowStateSetting.IsWindowOpen())
        {
            ExDebug.LogWarning("Window is not open, skipping restart.");
            return;
        }

        ExDebug.Log("Restarting window...");

        CurrentWindow.Close();
        ShowWindow<EdixorWindow>("EdixorWindow");
    }

    public void MinimizeWindow()
    {
        if (position.width > minimalSizeThreshold.x && position.height > minimalSizeThreshold.y)
        {
            originalWindowRect = position;
            WindowStateSetting.SetOriginalWindowRect(originalWindowRect);
        }

        WindowStateSetting.SetMinimized(true);
        minSize = minimalSizeThreshold;
        CurrentWindow.position = new Rect(position.x, position.y, minimalSizeThreshold.x, minimalSizeThreshold.y);
        ExDebug.Log("Window minimized to minimal size: " + minimalSizeThreshold);
    }

    public void ReturnWindowToOriginalSize()
    {
        position = originalWindowRect;
        WindowStateSetting.SetOriginalWindowRect(originalWindowRect);
        WindowStateSetting.SetMinimized(false);
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

        setting.OnWindowClose();
        WindowStateSetting.SetWindowOpen(false);
        CurrentWindow = null;
        ExDebug.Log($"{_title} is disable");
        ExDebug.EndGroup();
    }
}
