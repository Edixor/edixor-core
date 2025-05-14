using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using ExTools;

public class ExtensionManagerTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Extensions")]
    public static void ShowTab() => ShowTab<ExtensionManagerTab>();

    private VisualElement _downloadedContainer, _availableContainer;
    private ScrollView _downloadedList, _availableList;
    private TextField _searchDownloaded, _searchAvailable;
    private Toggle _toggleDownloaded, _toggleAvailable;
    private IExtensionIndexProvider _provider;
    private IExtensionInstaller _installer;
    private List<IndexEntry> _allExtensions;
    private HashSet<string> _installedExtensions;
 
    public void Awake() {
        Option("Extension Manager", "auto", "auto");
        _provider = container.ResolveNamed<IExtensionIndexProvider>(ServiceNames.GitHubExtensionIndexProvider);
        _installer = container.ResolveNamed<IExtensionInstaller>(ServiceNames.GitHubExtensionInstaller);
    }

    public void Start()
    {
        // 1) Разрешаем провайдер/инсталлятор здесь
        _provider = container.ResolveNamed<IExtensionIndexProvider>(ServiceNames.GitHubExtensionIndexProvider);
        _installer = container.ResolveNamed<IExtensionInstaller>(ServiceNames.GitHubExtensionInstaller);
        if (_provider == null) Debug.LogError("ExtensionManagerTab: _provider is null");
        if (_installer == null) Debug.LogError("ExtensionManagerTab: _installer is null");

        // 2) Разрешаем UI-элементы
        _toggleDownloaded    = root.Q<Toggle>("toggle-downloaded");
        _toggleAvailable     = root.Q<Toggle>("toggle-available");
        _downloadedContainer = root.Q<VisualElement>("downloaded-container");
        _availableContainer  = root.Q<VisualElement>("available-container");
        _searchDownloaded    = root.Q<TextField>("search-downloaded");
        _searchAvailable     = root.Q<TextField>("search-available");
        _downloadedList      = root.Q<ScrollView>("downloaded-list");
        _availableList       = root.Q<ScrollView>("available-list");

        // Проверяем, что всё нашли
        void Check(object ui, string name)
        {
            if (ui == null) Debug.LogError($"ExtensionManagerTab: cannot find UI element '{name}' in UXML");
        }
        Check(_toggleDownloaded,    "toggle-downloaded");
        Check(_toggleAvailable,     "toggle-available");
        Check(_downloadedContainer, "downloaded-container");
        Check(_availableContainer,  "available-container");
        Check(_searchDownloaded,    "search-downloaded");
        Check(_searchAvailable,     "search-available");
        Check(_downloadedList,      "downloaded-list");
        Check(_availableList,       "available-list");

        // 3) Тумблеры показывают/скрывают весь контейнер
        _toggleDownloaded.RegisterValueChangedCallback(evt =>
            _downloadedContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
        _toggleAvailable.RegisterValueChangedCallback(evt =>
            _availableContainer.style.display  = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);

        // 4) Поиск
        _searchDownloaded.RegisterValueChangedCallback(evt => RenderDownloaded());
        _searchAvailable.RegisterValueChangedCallback(evt => RenderAvailable());

        // 5) Собираем уже установленные расширения
        _installedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var extRoot = Path.Combine(Application.dataPath, "Extensions");
        if (Directory.Exists(extRoot))
            foreach (var d in Directory.GetDirectories(extRoot))
                _installedExtensions.Add(Path.GetFileName(d));

        // 6) Запускаем загрузку
        LoadExtensionsAsync();
    }

    private async void LoadExtensionsAsync()
    {
        try
        {
            if (_provider == null)
            {
                Debug.LogError("Cannot load extensions: _provider is null");
                return;
            }

            _allExtensions = await _provider.LoadIndexAsync();
            if (_allExtensions == null)
            {
                Debug.LogError("LoadExtensionsAsync: provider returned null list");
                return;
            }

            RenderDownloaded();
            RenderAvailable();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load extensions: {e}");
        }
    }

    private void RenderDownloaded()
    {
        _downloadedList.Clear();
        var filter = _searchDownloaded.value.ToLowerInvariant();
        int count = 0;

        foreach (var ext in _allExtensions)
        {
            if (!_installedExtensions.Contains(ext.name)) continue;
            if (!ext.name.ToLowerInvariant().Contains(filter)) continue;
            _downloadedList.Add(CreateExtensionItem(ext, false));
            count++;
        }

        if (count == 0)
        {
            var empty = new Label("You have no extensions here.");
            empty.AddToClassList("empty-label");
            _downloadedList.Add(empty);
        }
    }

    private void RenderAvailable()
    {
        _availableList.Clear();
        var filter = _searchAvailable.value.ToLowerInvariant();
        int count = 0;

        foreach (var ext in _allExtensions)
        {
            if (_installedExtensions.Contains(ext.name)) continue;
            if (!ext.name.ToLowerInvariant().Contains(filter)) continue;
            _availableList.Add(CreateExtensionItem(ext, true));
            count++;
        }

        if (count == 0)
        {
            var empty = new Label("You have no extensions here.");
            empty.AddToClassList("empty-label");
            _availableList.Add(empty);
        }
    }


    private VisualElement CreateExtensionItem(IndexEntry ext, bool canInstall)
    {
        // Root box
        var rootBox = new VisualElement();
        rootBox.AddToClassList("extension-box");

        // 1) Icon слева, по центру вертикали
        var iconWrapper = new VisualElement();
        iconWrapper.AddToClassList("extension-icon-wrapper");
        var img = new Image() { scaleMode = ScaleMode.ScaleToFit };
        img.AddToClassList("extension-icon");
        if (!string.IsNullOrEmpty(ext.iconUrl))
            LoadIcon(ext.iconUrl, img);
        else
            img.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icons/exten.png");
        iconWrapper.Add(img);
        rootBox.Add(iconWrapper);

        // 2) Основное содержимое — справа от иконки
        var contentWrapper = new VisualElement();
        contentWrapper.AddToClassList("extension-content-wrapper");

        // 2.1) Верхний бокс: имя слева, кнопка справа
        var topBox = new VisualElement();
        topBox.AddToClassList("extension-top-box");
        // Имя
        var titleLabel = new Label(ext.name);
        titleLabel.AddToClassList("extension-title");
        topBox.Add(titleLabel);
        // Установить / Удалить
        var actionBtn = new Button();
        actionBtn.AddToClassList("extension-action-btn");
        actionBtn.text = canInstall ? "Install" : "Uninstall";
        actionBtn.clicked += async () =>
        {
            if (canInstall)
            {
                await Install(ext);
            }
            else
            {
                await _installer.UninstallAsync(ext);
                _installedExtensions.Remove(ext.name);
                RenderDownloaded();
                RenderAvailable();
            }
        };
        topBox.Add(actionBtn);

        // 2.2) Нижний бокс: описание слева, Inspect справа
        var bottomBox = new VisualElement();
        bottomBox.AddToClassList("extension-bottom-box");
        // Описание
        var descLabel = new Label(ext.description);
        descLabel.AddToClassList("extension-desc");
        bottomBox.Add(descLabel);
        // Inspect
        var inspectBtn = new Button(() =>
        {
            // TODO: Inspector logic
        });
        inspectBtn.AddToClassList("extension-inspect-btn");
        inspectBtn.text = "Inspect";
        bottomBox.Add(inspectBtn);

        // Собираем контент
        contentWrapper.Add(topBox);
        contentWrapper.Add(bottomBox);
        rootBox.Add(contentWrapper);

        return rootBox;
    }


    private async Task Install(IndexEntry ext)
    {
        try
        {
            await _installer.InstallAsync(ext);
            _installedExtensions.Add(ext.name);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        finally
        {
            RenderDownloaded();
            RenderAvailable();
        }
    }

    private async void LoadIcon(string url, Image img)
    {
        using var req = UnityWebRequestTexture.GetTexture(url);
        var op = req.SendWebRequest(); while (!op.isDone) await Task.Yield();
        if (req.result == UnityWebRequest.Result.Success) img.image = DownloadHandlerTexture.GetContent(req);
        else img.image = EdixorObjectLocator.LoadObject<Texture2D>("Resources/Images/Icon/exten.png");
    }
}
