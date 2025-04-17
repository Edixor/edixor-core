using UnityEditor;
using UnityEngine;

public interface ICMItem
{
    string Name { get; }
    bool IsInteractive { get; set; }
    string interactiveKey { get; }

    void SaveState();

    void LoadState();

    void Draw();
}