using UnityEngine.UIElements;
using UnityEngine;
public abstract class UIContent 
{
    protected VisualElement root;
    public void Init(VisualElement root) {
        if (root == null)
        {
            Debug.LogError("Root element is null. Cannot initialize UIContent.");
            return;
        }
        this.root = root;
    }
    public abstract VisualElement LoadUI();
}
