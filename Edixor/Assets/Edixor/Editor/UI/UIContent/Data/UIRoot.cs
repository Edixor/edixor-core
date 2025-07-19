using UnityEngine.UIElements;
using UnityEngine;

using ExTools.UI;

namespace ExTools.UI.Content
{
    public class UIRoot : UIContent
    {
        private readonly StyleSetting StyleSetting;
        private readonly LayoutSetting layoutSetting;
        private EdixorDesign _design;


        public override VisualElement LoadUI()
        {
            _design = new EdixorDesign(
                styleData,
                layoutData,
                root,
                container
            );
            _design.LoadUI();
            var rootElement = new VisualElement();
            rootElement.Add(_design.GetRoot());
            return rootElement;
        }
    }
}
