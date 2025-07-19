using UnityEngine.UIElements;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

using ExTools;

namespace ExTools.Tabs.Advanced 
{
    public class ExtensionInspectTab : EdixorTab
    {
        private string _markdownUrl;
        private string _name;
        private string _version;
        private string _updatedDate;
        private string _description;
        private string _previewUrl;
        private string _avatarUrl;

        public ExtensionInspectTab(
            string previewUrl,
            string avatarUrl,
            string name,
            string version,
            string updatedDate,
            string description,
            string markdownUrl)
        {
            _markdownUrl = markdownUrl;
            _name = name;
            _version = version;
            _updatedDate = updatedDate;
            _description = description;
            _previewUrl = previewUrl;
            _avatarUrl = avatarUrl;
        }

        public void Awake()
        {
            Option("Extension Inspector", "auto", "auto");
        }

        public void Start()
        {
            Image previewImage = root.Q<Image>("preview-image");
            Image avatarImage = root.Q<Image>("avatar-image");
            Label nameLabel = root.Q<Label>("extension-name");
            Label versionLabel = root.Q<Label>("extension-version");
            Label dateLabel = root.Q<Label>("extension-date");
            Label descriptionLabel = root.Q<Label>("extension-description");

            nameLabel.text = _name;
            versionLabel.text = $"Version: {_version}";
            dateLabel.text = $"Updated: {_updatedDate}";
            descriptionLabel.text = _description;

            if (!string.IsNullOrEmpty(_previewUrl))
                LoadImage(_previewUrl, previewImage);
            if (!string.IsNullOrEmpty(_avatarUrl))
                LoadImage(_avatarUrl, avatarImage);
        }

        private async void LoadImage(string url, Image target)
        {
            UnityWebRequest req = UnityWebRequestTexture.GetTexture(url);
            UnityWebRequestAsyncOperation op = req.SendWebRequest(); while (!op.isDone) await Task.Yield();

            if (req.result == UnityWebRequest.Result.Success)
                target.image = DownloadHandlerTexture.GetContent(req);
            else
                target.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/exten.png");
        }
    }

}
