using UnityEngine.UIElements;
using UnityEngine;

[CreateAssetMenu(fileName = "EdixorWindowSettings", menuName = "Edixor/WindowSettings", order = 1)]
public class WindowSaveAsset : ScriptableObject
{
    public Rect originalWindowRect;
    public bool isWindowOpen;
    public bool isMinimized;
    public bool isMaximized;
    public VisualElement rootElement;
}
