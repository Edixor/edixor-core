using UnityEngine;

[System.Serializable]
public class LabelStyleEntry : IStyleEntry<ExLabelStyle>
{
    [SerializeField] private string name;
    [SerializeField] private ExLabelStyle style;
    public string Name => name;
    public ExLabelStyle Style => style;
    public LabelStyleEntry(string name)
    {
        this.name = name;
        style = new ExLabelStyle();
    }
}
