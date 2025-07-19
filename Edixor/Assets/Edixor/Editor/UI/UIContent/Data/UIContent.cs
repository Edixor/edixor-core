using UnityEngine.UIElements;
using ExTools;
public abstract class UIContent 
{
    protected VisualElement root;
    protected LayoutData layoutData;
    protected StyleData styleData;
    protected DIContainer container;
    public void Init(VisualElement root, LayoutData Layout, StyleData Style, DIContainer container)
    {
        if (container == null)
        {
            ExDebug.LogError("DIContainer is null. Cannot initialize UIContent.");
            return;
        }
        this.container = container;

        if (Layout == null)
        {
            ExDebug.LogError("LayoutData is null. Cannot initialize UIContent.");
            return;
        }
        
        if (Style == null)
        {
            ExDebug.LogError("StyleData is null. Cannot initialize UIContent.");
            return;
        }
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
