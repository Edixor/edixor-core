using UnityEngine.UIElements;
public interface IUIController
{
    void InitRoot(VisualElement root);
    public void Show(UIContent content);
    VisualElement GetElement(string name);
}