using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class WindowConfigurationVersion
{
    private readonly List<string> _functionNames = new List<string>();
    private readonly List<string> _hotKeyNames = new List<string>();
    private readonly List<string> _statusNames = new List<string>();
    private readonly List<string> _otherNames = new List<string>();
    private string _layoutName;
    private string _styleName;
    private string _Id;

    public void AddFunction(string functionName)
    {
        if (!_functionNames.Contains(functionName))
            _functionNames.Add(functionName);
    }

    public void AddStatus(string statusName)
    {
        if (!_statusNames.Contains(statusName))
            _statusNames.Add(statusName);
    }

    public void AddHotKey(string hotKeyName)
    {
        if (!_hotKeyNames.Contains(hotKeyName))
            _hotKeyNames.Add(hotKeyName);
    }

    public void AddOther(string name)
    {
        if (!_otherNames.Contains(name))
            _otherNames.Add(name);
    }

    public void SetLayout(string layoutName)
    {
        _layoutName = layoutName;
    }

    public void SetStyle(string styleName)
    {
        _styleName = styleName;
    }

    public void SetId(string Id)
    {
        _Id = Id;
    }

    private IEnumerable<string> AllElements()
    {
        var list = new List<string>();
        list.AddRange(_functionNames);
        list.AddRange(_hotKeyNames);
        list.AddRange(_statusNames);
        list.AddRange(_otherNames);
        if (!string.IsNullOrEmpty(_layoutName)) list.Add($"Layout:{_layoutName}");
        if (!string.IsNullOrEmpty(_styleName)) list.Add($"Style:{_styleName}");
        if (!string.IsNullOrEmpty(_Id)) list.Add($"Window:{_Id}");
        return list.OrderBy(n => n);
    }

    public string ComputeHash()
    {
        var all = string.Join(";", AllElements());
        using var md5 = MD5.Create();
        byte[] data = Encoding.UTF8.GetBytes(all);
        var hash = md5.ComputeHash(data);
        return new Guid(hash).ToString();
    }
}