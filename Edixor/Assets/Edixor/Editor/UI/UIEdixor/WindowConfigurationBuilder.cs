using UnityEngine;
using ExTools;
using ExTools.Settings;

public class WindowConfigurationBuilder
{
    private readonly IControllersFacade _controllers;
    private readonly ISettingsFacade _settings;
    private readonly string _Id;
    private readonly WindowConfigurationVersion _version;
    public TabData BasicTabInstance { get; private set; }

    public WindowConfigurationBuilder(
        IControllersFacade controllers,
        ISettingsFacade settings,
        string Id,
        WindowConfigurationVersion version)
    {
        _controllers = controllers;
        _settings = settings;
        _Id = Id;
        _version = version;

        _version.SetId(Id);
    }

    public WindowConfigurationBuilder HotKey(string name)
    {
        _controllers.LoadHotKey(name, _Id);
        _version.AddHotKey(name);
        return this;
    }

    public WindowConfigurationBuilder Function(string name)
    {
        _controllers.LoadFunction(name, name);
        _version.AddFunction(name);
        return this;
    }

    public WindowConfigurationBuilder Status(string name)
    {
        _controllers.LoadStatus(name, name);
        _version.AddStatus(name);
        return this;
    }

    public WindowConfigurationBuilder Other(string name)
    {
        _version.AddOther(name);
        return this;
    }

    public WindowConfigurationBuilder Layout(string layoutName)
    {
        _version.SetLayout(layoutName);
        return this;
    }

    public WindowConfigurationBuilder Style(string styleName)
    {
        _version.SetStyle(styleName);
        return this;
    }

    public WindowConfigurationBuilder BasicTab(string tabName)
    {
        if (!TabRegistry.TryGetType(tabName, out var type))
        {
            Debug.LogError($"[WindowConfigurationBuilder] Failed to find EdixorTab by name: {tabName}");
            return this;
        }

        var data = EdixorTab.CreateTabData(type);
        BasicTabInstance = data;
        _controllers.BasicTab(data);
        return this;
    }
}
