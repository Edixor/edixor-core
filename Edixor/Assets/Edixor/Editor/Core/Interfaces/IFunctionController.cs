using UnityEngine.UIElements;
public interface IFunctionController
{
    void Initialize(IUIController uiBase = null);
    void RestoreFunction();
    void AddFunction(Function function);
    void RemoveFunction(string functionId);
    void Execute(string functionId);
}
