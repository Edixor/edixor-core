using UnityEngine.UIElements;
using UnityEngine;
using System;
public interface IEdixorInterfaceFacade
{
    void InitOptions(VisualElement root, DIContainer container);
    void Initialize();
    void LoadHotKey(string name, string title = null, Action action = null);
    void LoadFunction(string name, string container = null, Action action = null);
    void OnKeys();
    void Update();
    void OnWindowClose();
    void AddTab(EdixorTab tab, bool saveState = true, bool autoSwitch = true);
    void SetLayout(string name);
    void SetStyle(string name);
    void BeginWindowSize(Vector2 size);
    void FixedWindowSize(Vector2 size);
    void BeginWindowPosition(Vector2 position);
    void FixedWindowPosition(Vector2 position);
}
