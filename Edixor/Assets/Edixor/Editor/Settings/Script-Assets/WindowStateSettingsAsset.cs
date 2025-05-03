using UnityEngine.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "WindowStateSettings", menuName = "Edixor/Settings/WindowSettings", order = 1)]
public class WindowStateSettingsAsset : ScriptableObject
{
    public Rect originalWindowRect;
    public bool isWindowOpen;
    public bool isMinimized;
    public bool isMaximized;
    public VisualElement rootElement;
}
