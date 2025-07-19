using UnityEngine.UIElements;
using UnityEngine;
using System;

using ExTools.UI.Content;
using ExTools.Settings;
using ExTools;

public interface IControllersFacade
{
    void InitOptions(VisualElement root, DIContainer container, ISettingsFacade settings);
    void Initialize();
    void ShowUI(UIContent content);
    void LoadHotKey(string name, string title = null, Action action = null);
    void LoadFunction(string name, string container = null, Action action = null);
    void LoadStatus(string name, string container = null, Action action = null);
    void AddFunction(Function function);
    void OnKeys();
    void Update();
    void OnWindowClose();
    void AddTab(TabData tab, bool saveState = true, bool autoSwitch = true);
    void AddTab(EdixorTab tab, bool saveState = true, bool autoSwitch = true);
    void BasicTab(TabData tab);
    EdixorTab GetLogicFor(TabData data);
    void ResetConfiguration();
}
