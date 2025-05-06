using UnityEngine.UIElements;
using ExTools;
public abstract class UIContent 
{
    protected VisualElement root;
    protected LayoutData layoutData;
    protected StyleData styleData;
    public void Init(VisualElement root, LayoutData Layout, StyleData Style) {
        if (root == null)
        {
            ExDebug.LogError("Root element is null. Cannot initialize UIContent.");
            return;
        }
        this.root = root;

        this.layoutData = Layout;
        this.styleData = Style;
    }
    public abstract VisualElement LoadUI();
}
