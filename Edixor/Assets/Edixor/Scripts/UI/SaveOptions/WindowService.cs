using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class WindowStateService : EdixorSettingTest<WindowSaveAsset>
{
    public WindowStateService() : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorWindowSettings.asset")) { }

    public bool IsWindowOpen() => settings.isWindowOpen;

    public void SetWindowOpen(bool open)
    {
        settings.isWindowOpen = open;
        Save();
    }

    public Rect GetOriginalWindowRect() => settings.originalWindowRect;

    public void SetOriginalWindowRect(Rect rect)
    {
        settings.originalWindowRect = rect;
        Save();
    }

    public VisualElement GetRootElement()
    {
        if (settings.rootElement == null)
        {
            Debug.LogError("Root element is null.");
            return null;
        }

        return settings.rootElement;
    }
    public void SetRootElement(VisualElement root)
    {
        if (root == null)
        {
            Debug.LogError("Root element is null.");
            return;
        }

        settings.rootElement = root;
    }

    public bool GetMinimized() {
        return settings.isMinimized;
    }

    public void SetMinimized(bool minimized) {
        settings.isMinimized = minimized;
    }
}
