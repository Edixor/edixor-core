using UnityEngine; 
using UnityEngine.UIElements;

using ExTools.Edixor.Interface;

namespace ExTools.UI.Content
{
    public class UIMin : UIContent
    {
        public override VisualElement LoadUI()
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;
            root.style.alignItems = Align.Center;
            root.style.justifyContent = Justify.Center;
            root.style.paddingTop = 5;
            root.style.paddingBottom = 2;

            var label = new Label("Window is in minimized mode, restore to original size?");
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
            label.style.marginBottom = 10;
            label.style.fontSize = 10;
            label.style.whiteSpace = WhiteSpace.Normal;
            label.style.flexShrink = 1;
            label.style.flexGrow = 1;
            root.Add(label);

            var button = new Button(() =>
            {
                container.ResolveNamed<IMinimizable>(container.Resolve<ServiceNameResolver>().Edixor_Mini).ReturnWindowToOriginalSize();
            })
            {
                text = "Return"
            };
            button.style.alignSelf = Align.Center;
            root.Add(button);

            return root;
        }
    }
}
