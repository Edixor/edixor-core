using ExTools;

public interface IStatusController
{
    void Process();
    void Initialize(IUIController ui, DIContainer container = null);
    void ResetConfiguration();
    void OnGUI();
    void AddItem(Status status);
    void RemoveItem(Status status);
}
