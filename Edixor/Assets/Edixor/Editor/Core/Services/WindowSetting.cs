using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

public class WindowStateSetting : EdixorSettingData<WindowStateSettingsAsset>
{
    public WindowStateSetting() : base("SettingAsset/WindowStateSettings.asset") { }

    public bool IsWindowOpen() => Settings.isWindowOpen;

    public void SetWindowOpen(bool open)
    {
        Settings.isWindowOpen = open;
        SaveSettings();
    }

    public Rect GetOriginalWindowRect() => Settings.originalWindowRect;

    public void SetOriginalWindowRect(Rect rect)
    {
        Settings.originalWindowRect = rect;
        SaveSettings();
    }

    public VisualElement GetRootElement()
    {
        if (Settings.rootElement == null)
        {
            Debug.LogError("Root element is null.");
            return null;
        }

        return Settings.rootElement;
    }
    public void SetRootElement(VisualElement root)
    {
        if (root == null)
        {
            Debug.LogError("Root element is null.");
            return;
        }

        Settings.rootElement = root;
    }

    public bool GetMinimized() {
        return Settings.isMinimized;
    }

    public void SetMinimized(bool minimized) {
        Settings.isMinimized = minimized;
    }
}
