using System;
using System.IO;
using System.Linq;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using UnityEngine;
using UnityEngine.UIElements;

public static class MarkdownUIRenderer
{
    public static VisualElement Render(string markdownText)
    {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;

        MarkdownDocument document = Markdown.Parse(markdownText);

        foreach (Block block in document)
        {
            VisualElement element = RenderBlock(block);
            if (element != null)
            {
                container.Add(element);
            }
        }

        return container;
    }

    private static VisualElement RenderBlock(Block block)
    {
        if (block is HeadingBlock heading)
        {
            return RenderHeading(heading);
        }

        if (block is ParagraphBlock paragraph)
        {
            return RenderParagraph(paragraph);
        }

        if (block is ListBlock list)
        {
            return RenderList(list);
        }

        if (block is QuoteBlock quote)
        {
            return RenderQuote(quote);
        }

        if (block is ThematicBreakBlock)
        {
            VisualElement hr = new VisualElement();
            hr.style.height = 1;
            hr.style.backgroundColor = Color.gray;
            hr.style.marginTop = 6;
            hr.style.marginBottom = 6;
            return hr;
        }

        return null;
    }

    private static VisualElement RenderHeading(HeadingBlock heading)
    {
        string text = GetInlineText(heading.Inline);
        Label label = new Label(text);

        label.style.unityFontStyleAndWeight = FontStyle.Bold;

        switch (heading.Level)
        {
            case 1:
                label.style.fontSize = 22;
                break;
            case 2:
                label.style.fontSize = 20;
                break;
            case 3:
                label.style.fontSize = 18;
                break;
            default:
                label.style.fontSize = 16;
                break;
        }

        label.style.marginBottom = 4;
        return label;
    }

    private static VisualElement RenderParagraph(ParagraphBlock paragraph)
    {
        string text = GetInlineText(paragraph.Inline);
        Label label = new Label(text);
        label.style.whiteSpace = WhiteSpace.Normal;
        label.style.marginBottom = 4;
        return label;
    }

    private static VisualElement RenderList(ListBlock list)
    {
        VisualElement container = new VisualElement();
        container.style.flexDirection = FlexDirection.Column;
        container.style.marginBottom = 6;

        foreach (ListItemBlock item in list)
        {
            foreach (Block child in item)
            {
                if (child is ParagraphBlock para)
                {
                    string bullet = list.IsOrdered ? "• " : "• ";
                    string text = bullet + GetInlineText(para.Inline);
                    Label label = new Label(text);
                    label.style.marginLeft = 10;
                    label.style.marginBottom = 2;
                    container.Add(label);
                }
            }
        }

        return container;
    }

    private static VisualElement RenderQuote(QuoteBlock quote)
    {
        VisualElement container = new VisualElement();
        container.style.borderLeftWidth = 2;
        container.style.borderLeftColor = Color.gray;
        container.style.paddingLeft = 6;
        container.style.marginBottom = 4;

        foreach (Block child in quote)
        {
            VisualElement inner = RenderBlock(child);
            if (inner != null)
            {
                container.Add(inner);
            }
        }

        return container;
    }

    private static string GetInlineText(ContainerInline container)
    {
        if (container == null)
            return "";

        string result = "";

        foreach (Inline inline in container)
        {
            if (inline is LiteralInline literal)
            {
                result += literal.Content.Text.Substring(literal.Content.Start, literal.Content.Length);
            }
            else if (inline is EmphasisInline emphasis)
            {
                string inner = GetInlineText(emphasis);
                result += emphasis.DelimiterChar == '*' ? $"<b>{inner}</b>" : inner;
            }
            else if (inline is LineBreakInline)
            {
                result += "\n";
            }
        }

        return result;
    }
}
