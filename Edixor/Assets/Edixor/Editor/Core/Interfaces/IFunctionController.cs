using UnityEngine.UIElements;
public interface IFunctionController
{
    void InitFunction(VisualElement root = null);
    void AddFunction(Function function);
    void RemoveFunction(string functionId);
    void Execute(string functionId);
}
