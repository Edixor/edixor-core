public interface IFunctionController
{
    void AddFunction();
    void RemoveFunction(string functionId);
    void Execute(string functionId);
}
