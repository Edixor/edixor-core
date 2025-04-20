using UnityEngine;

[CreateAssetMenu(fileName = "EdixorParameters", menuName = "Edixor/Style/EdixorParameters", order = 1)]
public class EdixorParameters: StyleParameters
{
    [SerializeField] private Color functionIconColors;
    [SerializeField] private Color functionBackgroundColors;
    [SerializeField] private Color functionBorderColors;
    [SerializeField] private Color tabBackgroundColor;
    [SerializeField] private Color tabTextColor;
    [SerializeField] private Color tabBorderColor;
    [SerializeField] private Color tabIconColor;
    public Color FunctionIconColors => functionIconColors;
    public Color FunctionBackgroundColors => functionBackgroundColors;
    public Color FunctionBorderColors => functionBorderColors;
    public Color TabBackgroundColor => tabBackgroundColor;
    public Color TabTextColor => tabTextColor;
    public Color TabBorderColor => tabBorderColor;
    public Color TabIconColor => tabIconColor;

}
