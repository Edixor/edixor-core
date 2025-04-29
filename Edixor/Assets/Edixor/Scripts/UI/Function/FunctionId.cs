
public struct FunctionId
{
    public string container;
    public string name;

    public FunctionId(string container, string name)
    {
        this.container = container ?? "NoneContainer";
        this.name = name ?? "NoneName";
    }
}