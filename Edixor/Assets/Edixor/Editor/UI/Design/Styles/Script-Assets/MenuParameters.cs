using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MenuParameters", menuName = "Edixor/Style/MenuParameters", order = 2)]
public class MenuParameters : StyleParameters
{
    [SerializeField] private Color menuBackgroundColor;
    [SerializeField] private Color itemHoverColor;
    [SerializeField] private Color itemSelectedColor;
    public Color MenuBackgroundColor => menuBackgroundColor;
    public Color ItemHoverColor => itemHoverColor;
    public Color ItemSelectedColor => itemSelectedColor;
}
