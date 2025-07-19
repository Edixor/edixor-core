using UnityEngine.UIElements;
using ExTools.Tabs.Basic;
using UnityEngine;
using System;

public class NewsUIFactory
{
    public static VisualElement CreateNewsElement(NewsEntry news, Texture2D previewTex, Action<NewsEntry> onContinue)
    {
        var item = new VisualElement();
        item.AddToClassList("news-box");
        item.AddToClassList("E-box");

        if (previewTex != null)
        {
            var img = new Image { image = previewTex, scaleMode = ScaleMode.ScaleToFit };
            img.AddToClassList("news-preview");
            img.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var w = img.resolvedStyle.width;
                img.style.height = w > 0 ? w / 3f : 0;
            });
            item.Add(img);
        }

        var bottom = new VisualElement();
        bottom.AddToClassList("news-bottom");
        bottom.Add(new Label(news.title) { name = "news-title" });

        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.justifyContent = Justify.SpaceBetween;
        row.style.width = Length.Percent(100);
        row.Add(new Label(news.date) { name = "news-date" });

        var cont = new Label("continuation...") { name = "news-continue" };
        cont.AddToClassList("E-link");
        cont.RegisterCallback<ClickEvent>(_ => onContinue?.Invoke(news));
        row.Add(cont);

        bottom.Add(row);
        item.Add(bottom);

        return item;
    }
}
