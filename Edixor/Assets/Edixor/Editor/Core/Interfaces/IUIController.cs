using UnityEngine.UIElements;
public interface IUIController
{
    void InitRoot(VisualElement root);
    void SetStyle(StyleData style);
    void SetLayout(LayoutData layout);
    public void Show(UIContent content);
    VisualElement GetElement(string name);
}