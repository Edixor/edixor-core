[System.Serializable]
public class ScrollStyleEntry : IScrollStyleEntry
{
    public string Name { get; private set; }
    public ExScrollStyle Style { get; private set; }

    public ScrollStyleEntry(string name)
    {
        Name = name;
    }
}
