using System.Collections.Generic; 
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using ExTools;
using System;

public class EdixorInspector : Editor, IRestartable, IClosable
{
    private static readonly HashSet<Type> _initializedInspectorTypes = new();
    protected DIContainer container;
    protected IEdixorInterfaceFacade setting;
    private VisualElement _root;

    public override VisualElement CreateInspectorGUI()
    {
        container = LoadOrCreateContainer();
        RegisterServices();

        _root = new VisualElement
        {
            focusable = true,
            pickingMode = PickingMode.Position
        };

        setting = container.ResolveNamed<IEdixorInterfaceFacade>(ServiceNames.EdixorUIManager_EdixorInspector);
        setting.InitOptions(_root, container);

        if (_initializedInspectorTypes.Add(GetType()))
            OnOptions();

        setting.Initialize();

        _root.RegisterCallback<KeyDownEvent>(_ => setting.OnKeys());
        FileDragHandler.RegisterDragHandlers(_root, OnTabAddedFromDrag);

        return _root;
    }

    protected virtual void OnOptions() { }

    private static DIContainer LoadOrCreateContainer()
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

    private void RegisterServices()
    {
        container.RegisterNamed<IClosable>(ServiceNames.IClosable_EdixorInspector, this);
        container.RegisterNamed<IRestartable>(ServiceNames.IRestartable_EdixorInspector, this);
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

    public void RestartWindow()
    {
        if (_root == null) return;

        _root.Clear();

        setting?.InitOptions(_root, container);
        setting?.Initialize();

        FileDragHandler.RegisterDragHandlers(_root, OnTabAddedFromDrag);
        _root.RegisterCallback<KeyDownEvent>(_ => setting.OnKeys());

        ExDebug.Log("EdixorInspector restarted");
    }

    public void CloseWindow()
    {
        // Условно "закрываем" инспектор: чистим всё, вызываем OnWindowClose, но оставить объект в редакторе.
        _root?.Clear();
        setting?.OnWindowClose();
        ExDebug.Log("EdixorInspector closed (UI cleared)");
    }


    private void OnDisable()
    {
        setting?.OnWindowClose();
    }
}
