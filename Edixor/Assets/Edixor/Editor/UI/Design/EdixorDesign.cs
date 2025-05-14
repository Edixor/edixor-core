using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using ExTools;

public class EdixorDesign
{
    public StyleData Style { get; private set; }
    public LayoutData Layout { get; private set; }
    private VisualTreeAsset _tree;
    private StyleSheet _layoutSheet;
    private StyleLogicEdixor _styleLogic;
    private EdixorParameters _parameters;
    private VisualElement _root;
    private DIContainer _container;

    public EdixorDesign(StyleData style, LayoutData layout, VisualElement root, DIContainer container)
    {
        Style      = style;
        Layout     = layout;
        _root      = root;
        _container = container;
    }
    public EdixorDesign(DIContainer container) => _container = container;
    public EdixorDesign(StyleData style, LayoutData layout) { Style = style; Layout = layout; }

    public void LoadUI(bool demo = false)
    {
        if (_root == null)
            _root = new VisualElement();

        // 1) Instantiate UXML/USS
        _tree        = Layout.LayoutVisualTreeAsset;
        _layoutSheet = Layout.LayoutStyleSheet;
        if (_tree == null)
        {
            ExDebug.LogError("Missing UXML");
            return;
        }

        _root.Clear();
        var instance = _tree.Instantiate();
        instance.style.flexGrow = 1;
        instance.style.width    = Length.Percent(100);
        instance.style.height   = Length.Percent(100);
        _root.Add(instance);

        if (_layoutSheet != null && !_root.styleSheets.Contains(_layoutSheet))
            _root.styleSheets.Add(_layoutSheet);

        // 2) Init style logic
        _parameters = (EdixorParameters)Style.AssetParameters[0];
        _styleLogic = new StyleLogicEdixor(_root, _parameters);
        _styleLogic.Init();

        _parameters.ScrollStyle.ApplyScrollStyleWithStates(_root);

        // 4) Register style logic
        _container?.Register<StyleLogicEdixor>(_styleLogic);

        // 5) Demo block
        if (demo)
        {
            foreach (var elem in Layout.AssetParameters.Elements)
            {
                if (elem?.functionNames == null) continue;
                var section = GetSection(elem.elementName);
                if (section == null) continue;
                foreach (var nm in elem.functionNames)
                {
                    var b = new Button(() => { }) { text = "F" };
                    b.AddToClassList("function");
                    _parameters.FunctionStyle.ApplyWithStates(b);
                    section.Add(b);
                }
            }
        }

        // 6) Tabs section
        var tabs = new TabUIFactory().CreateTabContainer(new NewTab(), _parameters, () => { }, () => { });
        GetSection("tab-section")?.Add(tabs);
        _parameters.AddTabStyle.ApplyWithStates(
            GetSection("tab-section").Q<VisualElement>("AddTab"));
    }

    public VisualElement GetSection(string name) => _root.Q(name);
    public VisualElement GetRoot()            => _root;
}
