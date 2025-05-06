using UnityEngine.UIElements;
using UnityEngine;
public class UIRoot : UIContent
{
    private readonly StyleSetting StyleSetting;
    private readonly LayoutSetting layoutSetting;
    private readonly DIContainer container;
    private EdixorDesign design;

    public UIRoot(DIContainer container)
    {
        this.container = container;
    }

    public override VisualElement LoadUI()
    {
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
