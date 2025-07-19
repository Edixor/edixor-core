using UnityEngine; 
using UnityEngine.UIElements;

public class EmptyUI : UIContent
{
    public override VisualElement LoadUI()
    {
        VisualElement root = new VisualElement();
        root.style.flexDirection = FlexDirection.Column;
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        root.style.paddingTop = 40;
        root.style.paddingLeft = 20;
        root.style.paddingRight = 20;

        Label title = new Label("Looks like you closed all the tabs =[");
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.fontSize = 26;
        title.style.marginBottom = 12;
        title.style.unityTextAlign = TextAnchor.UpperLeft;
        root.Add(title);

        Label subtitle = new Label("What do you want to do now?");
        subtitle.style.fontSize = 16;
        subtitle.style.marginBottom = 24;
        subtitle.style.unityTextAlign = TextAnchor.UpperLeft;
        root.Add(subtitle);

        VisualElement CreateButton(string text)
        {
            var btn = new Button { text = text };
            btn.style.width = 320;
            btn.style.height = 36;
            btn.style.marginBottom = 10;
            btn.style.fontSize = 15;
            btn.style.unityFontStyleAndWeight = FontStyle.Normal;
            return btn;
        }

        root.Add(CreateButton("Create a new tab"));
        root.Add(CreateButton("Open documentation"));
        root.Add(CreateButton("Open settings"));
        root.Add(CreateButton("Download new plugin or extensions"));
        root.Add(CreateButton("Exit"));

        return root;
    }
}
