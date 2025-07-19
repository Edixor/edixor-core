using UnityEngine;

[System.Serializable]
public class BoxStyleEntry : IStyleEntry<ExBoxStyle>
{
    [SerializeField] private string name;
    [SerializeField] private ExBoxStyle style;

    public string Name => name;
    public ExBoxStyle Style => style;

    public BoxStyleEntry(string name)
    {
        this.name = name;
        style = new ExBoxStyle();
    }
}
