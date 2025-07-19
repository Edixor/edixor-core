using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;
using UnityEditor;

using ExTools.Tabs.Basic;
using ExTools;

namespace ExTools.UI
{
    public class EdixorDesign
    {
        public StyleData Style { get; private set; }
        public LayoutData Layout { get; private set; }
        private VisualTreeAsset _tree;
        private StyleSheet _layoutSheet;
        private EdixorParameters _parameters;
        private VisualElement _root;
        private DIContainer _container;

        public EdixorDesign(StyleData style, LayoutData layout, VisualElement root, DIContainer container)
        {
            Style = style;
            Layout = layout;
            _root = root;
            _container = container;
        }
        public EdixorDesign(DIContainer container) => _container = container;
        public EdixorDesign(StyleData style, LayoutData layout) { Style = style; Layout = layout; }

        public void LoadUI(bool demo = false)
        {
            if (_root == null)
                _root = new VisualElement();


            _tree = Layout.LayoutVisualTreeAsset;
            _layoutSheet = Layout.LayoutStyleSheet;
            if (_tree == null)
            {
                ExDebug.LogError("Missing UXML");
                return;
            }

            _root.Clear();
            var instance = _tree.Instantiate();
            instance.style.flexGrow = 1;
            instance.style.width = Length.Percent(100);
            instance.style.height = Length.Percent(100);
            _root.Add(instance);

            if (_layoutSheet != null && !_root.styleSheets.Contains(_layoutSheet))
                _root.styleSheets.Add(_layoutSheet);

            _parameters = Style.GetEdixorParameters();

            _parameters.Scroll.ApplyScrollStyleWithStates(_root);

            foreach (var entry in _parameters.Layout)
            {
                var section = GetSection(entry.Name);
                if (section != null)
                    entry.Style.ApplyWithStates(section);
            }

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
                        _parameters.GetStyleByName(_parameters.Functions, "normal").ApplyWithStates(b);
                        section.Add(b);
                    }
                }
            }

            NewTab tab = new NewTab();
            tab.Initialize(GetSection("middle-section-content"), _container);
            tab.InvokeAwake();
            tab.EnsureTabOpensLoaded();

            ExTabStyle activeStyle = _parameters.GetStyleByName(_parameters.Tabs, "active");

            var tabs = new TabUIFactory().CreateTabContainer(tab, activeStyle, () => { }, () => { });

            GetSection("tab-section")?.Add(tabs);
            _parameters.AddTabButton.Style.ApplyWithStates(
                GetSection("tab-section").Q<VisualElement>("AddTab"));
        }

        public VisualElement GetSection(string name) => _root.Q(name);
        public VisualElement GetRoot() => _root;
    }

}
