using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using ExTools;

namespace ExTools.Tabs.Advanced
{
    public class NewsTab : EdixorTab
    {
        public string NewsTitle;
        public string Date;
        public string PreviewImagePath;
        public string MarkdownPath;

        public NewsTab(string markdownPath, string title, string date, string previewImagePath)
        {
            NewsTitle = title;
            Date = date;
            PreviewImagePath = previewImagePath;
            MarkdownPath = markdownPath;
        }

        private Image _previewImage;
        private Label _titleLabel;
        private Label _dateLabel;
        private VisualElement _contentContainer;

        public void Awake()
        {
            Option(string.IsNullOrEmpty(NewsTitle) ? "News: None" : "News: " + NewsTitle, "auto-advanced", "auto-advanced");
        }

        // public void Start()
        // {
        //     if (root == null)
        //     {
        //         ExDebug.LogError("root is null in NewsTab.Start — possibly Start() called before UXML loaded.");
        //         return;
        //     }

        //     _previewImage = root.Q<Image>("preview-image");
        //     _titleLabel = root.Q<Label>("title-label");
        //     _dateLabel = root.Q<Label>("date-label");
        //     _contentContainer = root.Q<VisualElement>("content-container");

        //     if (_titleLabel == null || _dateLabel == null || _previewImage == null || _contentContainer == null)
        //     {
        //         ExDebug.LogError("One or more UI elements not found in UXML.");
        //         return;
        //     }

        //     _titleLabel.text = string.IsNullOrEmpty(NewsTitle) ? "No Title" : NewsTitle;
        //     _dateLabel.text = string.IsNullOrEmpty(Date) ? "" : Date;

        //     LoadPreviewImage();

        //     if (!string.IsNullOrEmpty(MarkdownPath))
        //         LoadAndRenderMarkdown(MarkdownPath);
        //     else
        //         _contentContainer.Add(new Label("No content to display."));
        // }

        // private async void LoadPreviewImage()
        // {
        //     if (string.IsNullOrEmpty(PreviewImagePath))
        //     {
        //         _previewImage.image = null;
        //         return;
        //     }

        //     using var req = UnityWebRequestTexture.GetTexture(PreviewImagePath);
        //     var op = req.SendWebRequest();
        //     while (!op.isDone) await Task.Yield();

        //     if (req.result == UnityWebRequest.Result.Success)
        //         _previewImage.image = DownloadHandlerTexture.GetContent(req);
        //     else
        //         _previewImage.image = null;

        //     float width = _previewImage.resolvedStyle.width;
        //     if (width > 0f)
        //         _previewImage.style.height = width / 3f;
        // }

        // private async void LoadAndRenderMarkdown(string path)
        // {
        //     _contentContainer.Clear();
        //     string mdText = null;

        //     if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        //     {
        //         using var req = UnityWebRequest.Get(path);
        //         var op = req.SendWebRequest();
        //         while (!op.isDone) await Task.Yield();
        //         if (req.result == UnityWebRequest.Result.Success)
        //             mdText = req.downloadHandler.text;
        //     }
        //     else
        //     {
        //         string fullPath = Path.Combine(Application.dataPath, path);
        //         if (File.Exists(fullPath))
        //             mdText = await File.ReadAllTextAsync(fullPath);
        //     }

        //     if (string.IsNullOrEmpty(mdText))
        //     {
        //         _contentContainer.Add(new Label("Markdown file not found or empty."));
        //         return;
        //     }

        //     var rendered = MarkdownUIRenderer.Render(mdText);
        //     _contentContainer.Add(rendered);
        // }
    }
}
