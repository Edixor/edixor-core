using UnityEditor;
using UnityEngine;

public interface ICMItem
{
    string Name { get; }
    bool IsInteractive { get; set; }

    void SaveState();

    void LoadState();

    void Draw<T, I>(Menu<T, I> menu, float itemHeight, GUIStyle style) where T : Menu<T, I> where I : ICMItem;
}