using UnityEngine.UIElements;
using UnityEngine;
public abstract class UIContent 
{
    protected VisualElement root;
    protected LayoutData layoutData;
    protected StyleData styleData;
    public void Init(VisualElement root, LayoutData Layout, StyleData Style) {
        if (root == null)
        {
            Debug.LogError("Root element is null. Cannot initialize UIContent.");
            return;
        }
        this.root = root;

        Debug.Log("layoutData == null? " + (layoutData == null));
        Debug.Log("styleData  == null? " + (styleData  == null));

        this.layoutData = Layout;
        this.styleData = Style;
    }
    public abstract VisualElement LoadUI();
}
