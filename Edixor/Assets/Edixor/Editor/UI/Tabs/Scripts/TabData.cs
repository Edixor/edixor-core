using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Edixor/Tabs/Tab Data", fileName = "TabData")]
public class TabData : ScriptableObject
{
    [SerializeField] private string _logicTypeName;
    [SerializeField] private string _tabName;
    [SerializeField] private VisualTreeAsset _uxml;
    [SerializeField] private StyleSheet _uss;
    [SerializeField] private Texture2D _icon;

    public string LogicTypeName => _logicTypeName;
    public string TabName => _tabName;
    public VisualTreeAsset Uxml => _uxml;
    public StyleSheet Uss => _uss;
    public Texture2D Icon => _icon;

    public void Init(string logicTypeName, string tabName, VisualTreeAsset uxml, StyleSheet uss, Texture2D icon)
    {
        _logicTypeName = logicTypeName;
        _tabName = string.IsNullOrEmpty(tabName) ? "None" : tabName;
        _uxml = uxml;
        _uss = uss;
        _icon = icon;

        Debug.Log($"TabData.Init called with:\nlogicTypeName={logicTypeName}\ntabName={tabName}\n" +
              $"uxml={(uxml != null ? uxml.name : "null")}\nuss={(uss != null ? uss.name : "null")}\nicon={(icon != null ? icon.name : "null")}");
    }
}
