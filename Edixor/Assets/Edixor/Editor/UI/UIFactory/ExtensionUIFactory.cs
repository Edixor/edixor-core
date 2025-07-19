using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using ExTools;

public class ExtensionUIFactory
{
    private readonly Texture2D _defaultIcon;

    public ExtensionUIFactory()
    {
        _defaultIcon = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/exten.png");
    }

    public VisualElement CreateExtensionItem(
        ExtensionEntry ext,
        bool canInstall,
        Func<ExtensionEntry, Task> installCallback,
        Func<ExtensionEntry, Task> uninstallCallback,
        Action<ExtensionEntry> inspectCallback)
    {
        VisualElement rootBox = new VisualElement();
        rootBox.AddToClassList("extension-box");

        VisualElement iconWrapper = new VisualElement();
        iconWrapper.AddToClassList("extension-icon-wrapper");

        Image iconImage = new Image { scaleMode = ScaleMode.ScaleToFit };
        iconImage.AddToClassList("extension-icon");

        if (!string.IsNullOrEmpty(ext.iconUrl))
        {
            _ = LoadIconAsync(ext.iconUrl, iconImage);
        }
        else
        {
            iconImage.image = _defaultIcon;
        }

        iconWrapper.Add(iconImage);
        rootBox.Add(iconWrapper);

        VisualElement contentWrapper = new VisualElement();
        contentWrapper.AddToClassList("extension-content-wrapper");

        VisualElement topBox = new VisualElement();
        topBox.AddToClassList("extension-top-box");

        Label titleLabel = new Label(ext.name);
        titleLabel.AddToClassList("extension-title");
        topBox.Add(titleLabel);

        Button actionButton = new Button();
        actionButton.AddToClassList("extension-action-btn");
        actionButton.text = canInstall ? "Install" : "Uninstall";
        actionButton.clicked += async () =>
        {
            if (canInstall)
            {
                await installCallback(ext);
            }
            else
            {
                await uninstallCallback(ext);
            }
        };
        topBox.Add(actionButton);

        VisualElement bottomBox = new VisualElement();
        bottomBox.AddToClassList("extension-bottom-box");

        Label descriptionLabel = new Label(ext.description);
        descriptionLabel.AddToClassList("extension-desc");
        bottomBox.Add(descriptionLabel);

        Button inspectButton = new Button(() =>
        {
            inspectCallback?.Invoke(ext);
        });
        inspectButton.AddToClassList("extension-inspect-btn");
        inspectButton.text = "Inspect";
        bottomBox.Add(inspectButton);

        contentWrapper.Add(topBox);
        contentWrapper.Add(bottomBox);
        rootBox.Add(contentWrapper);

        return rootBox;
    }

    private async Task LoadIconAsync(string url, Image image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            image.image = DownloadHandlerTexture.GetContent(request);
        }
        else
        {
            image.image = _defaultIcon;
        }
    }
}
