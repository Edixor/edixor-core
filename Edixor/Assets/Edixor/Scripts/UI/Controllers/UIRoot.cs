using UnityEngine.UIElements;
using UnityEngine;
public class UIRoot : UIContent
{
    private readonly StyleService styleService;
    private readonly LayoutSetting layoutSetting;
    private readonly DIContainer container;
    private EdixorDesign design;

    public UIRoot(DIContainer container)
    {
        this.container = container;
    }

    public override VisualElement LoadUI()
    {
        Debug.Log("layoutData == null? " + (layoutData == null));
        Debug.Log("styleData  == null? " + (styleData  == null));

        design = new EdixorDesign(
            styleData,
            layoutData,
            root,
            container
        );
        design.LoadUI();
        var rootElement = new VisualElement();
        rootElement.Add(design.GetRoot());
        return rootElement;
    }
}
