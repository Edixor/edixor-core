public interface IFunctionController
{
    void Process();
    void Initialize(IUIController ui, DIContainer container = null);
    void ResetConfiguration();
    void AddItem(Function function);
    void RemoveItem(Function function);
    void Execute(string functionId);
}
