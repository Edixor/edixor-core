using UnityEngine.UIElements;
public class UIRoot : UIContent
{
    private readonly WindowStateService windowState;
    private readonly StyleService styleService;
    private readonly LayoutService layoutService;
    private readonly DIContainer container;

    private EdixorDesign design;

    public UIRoot(DIContainer container)
    {
        this.container = container;
        this.windowState = container.ResolveNamed<WindowStateService>(ServiceNames.WindowStateSetting);
        this.styleService = container.ResolveNamed<StyleService>(ServiceNames.StyleSetting);
        this.layoutService = container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting);
    }

    public override VisualElement LoadUI()
    {
        design = new EdixorDesign(
            styleService.GetCurrentItem(),
            layoutService.GetCurrentItem(),
            root,
            container
        );
        design.LoadUI();
        var rootElement = new VisualElement();
        rootElement.Add(design.GetRoot());
        return rootElement;
    }
}
