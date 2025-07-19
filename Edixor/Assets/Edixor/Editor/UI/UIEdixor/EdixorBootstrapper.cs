using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;
using System;

using ExTools.Settings;
using ExTools;

public static class EdixorBootstrapper
{
    public static void Bootstrap(
        IControllersFacade controllers,
        ISettingsFacade settings,
        VisualElement root,
        DIContainer container,
        Action configure)
    {
        controllers.InitOptions(root, container, settings);
        AttachInputCatcher(root, controllers);

        try { configure?.Invoke(); }
        catch (Exception ex)
        {
            ExDebug.LogError($"[Bootstrapper] Error in config: {ex}");
        }

        controllers.Initialize();

        container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting)
            .SetColdStart(false);       
    }

    private static void AttachInputCatcher(VisualElement root, IControllersFacade controllers)
    {
        IMGUIContainer catcher = new IMGUIContainer(() =>
        {
            Event e = Event.current;
            if (e == null) return;

            if (e.type == EventType.KeyDown || e.type == EventType.KeyUp)
            {
                controllers.OnKeys();
                e.Use();
            }
        });

        catcher.focusable = true;
        catcher.pickingMode = PickingMode.Position;
        catcher.style.position = Position.Absolute;
        catcher.style.width = 1;
        catcher.style.height = 1;

        root.Add(catcher);

        EditorApplication.delayCall += () => catcher.Focus();
    }
}
