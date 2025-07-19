
public struct HotKeyId
{
    public HotKeyId(string title, string name = null)
    {
        this.title = title ?? "NoneTitle";
        this.name = name ?? "NoneName";
    }

    public string title;
    public string name;
}
