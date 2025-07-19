using UnityEngine;

[System.Serializable]
public class ButtonStyleEntry : IStyleEntry<ExButtonStyle>
{
    [SerializeField] private string name;
    [SerializeField] private ExButtonStyle style;
    public string Name => name;
    public ExButtonStyle Style => style;
    public ButtonStyleEntry(string name)
    {
        this.name = name;
        style = new ExButtonStyle();
    }
}
