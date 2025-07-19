using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;

using ExTools.Tabs.Advanced;
using ExTools;

public class ExtensionManagerTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Extensions")]
    public static void ShowTab() => ShowTab<ExtensionManagerTab>();

    private VisualElement _downloadedContainer, _availableContainer;
    private ScrollView _downloadedList, _availableList;
    private TextField _searchDownloaded, _searchAvailable;
    private Toggle _toggleDownloaded, _toggleAvailable;
    private IGenericIndexProvider<ExtensionEntry> _provider;
    private IExtensionInstaller _installer;
    private List<ExtensionEntry> _allExtensions;
    private HashSet<string> _installedExtensions;
    private ExtensionUIFactory _uiFactory;
    private const string _providerUrl = "https://raw.githubusercontent.com/Terafy/edixor-extensions/main/index.json";

    public void Awake()
    {
        Option("Extension Manager", "auto", "auto", "Resources/Images/Icons/exten.png");
    }

    public void Start()
    {
        IExtensionIndexProviderFactory factory = container.Resolve<IExtensionIndexProviderFactory>();
        _provider = factory.Create<ExtensionEntry>(_providerUrl);

        _installer = container.ResolveNamed<IExtensionInstaller>(ServiceNameKeys.GitHubExtensionInstaller);

        _uiFactory = new ExtensionUIFactory();

        if (_provider == null) Debug.LogError("ExtensionManagerTab: _provider is null");
        if (_installer == null) Debug.LogError("ExtensionManagerTab: _installer is null");

        _toggleDownloaded    = root.Q<Toggle>("toggle-downloaded");
        _toggleAvailable     = root.Q<Toggle>("toggle-available");
        _downloadedContainer = root.Q<VisualElement>("downloaded-container");
        _availableContainer  = root.Q<VisualElement>("available-container");
        _searchDownloaded    = root.Q<TextField>("search-downloaded");
        _searchAvailable     = root.Q<TextField>("search-available");
        _downloadedList      = root.Q<ScrollView>("downloaded-list");
        _availableList       = root.Q<ScrollView>("available-list");

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

        _toggleDownloaded.RegisterValueChangedCallback(evt =>
            _downloadedContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
        _toggleAvailable.RegisterValueChangedCallback(evt =>
            _availableContainer.style.display  = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);

        _searchDownloaded.RegisterValueChangedCallback(evt => RenderDownloaded());
        _searchAvailable.RegisterValueChangedCallback(evt => RenderAvailable());

        _installedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string extRoot = Path.Combine(Application.dataPath, "Extensions");
        if (Directory.Exists(extRoot))
        {
            foreach (string directory in Directory.GetDirectories(extRoot))
            {
                _installedExtensions.Add(Path.GetFileName(directory));
            }
        }

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

            string basePath = GetBasePathFromUrl(_providerUrl); 
            foreach (ExtensionEntry ext in _allExtensions)
            {
                ext.ResolveUrls(basePath);
            }

            RenderDownloaded();
            RenderAvailable();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load extensions: {e}");
        }
    }

    private string GetBasePathFromUrl(string url)
    {
        int lastSlash = url.LastIndexOf('/');
        if (lastSlash < 0) return url;
        return url.Substring(0, lastSlash + 1);
    }

    private void RenderDownloaded()
    {
        _downloadedList.Clear();
        string filter = _searchDownloaded.value.ToLowerInvariant();
        int count = 0;

        foreach (ExtensionEntry ext in _allExtensions)
        {
            if (!_installedExtensions.Contains(ext.name)) continue;
            if (!ext.name.ToLowerInvariant().Contains(filter)) continue;

            _downloadedList.Add(_uiFactory.CreateExtensionItem(
                ext,
                false,
                Install,
                _installer.UninstallAsync,
                Inspect
            ));
            count++;
        }

        if (count == 0)
        {
            Label empty = new Label("You have no extensions here.");
            empty.AddToClassList("empty-label");
            _downloadedList.Add(empty);
        }
    }

    private void RenderAvailable()
    {
        _availableList.Clear();
        string filter = _searchAvailable.value.ToLowerInvariant();
        int count = 0;

        foreach (ExtensionEntry ext in _allExtensions)
        {
            if (_installedExtensions.Contains(ext.name)) continue;
            if (!ext.name.ToLowerInvariant().Contains(filter)) continue;

            _availableList.Add(_uiFactory.CreateExtensionItem(
                ext,
                true,
                Install,
                _installer.UninstallAsync,
                Inspect
            ));
            count++;
        }

        if (count == 0)
        {
            Label empty = new Label("You have no extensions here.");
            empty.AddToClassList("empty-label");
            _availableList.Add(empty);
        }
    }

    private async Task Install(ExtensionEntry ext)
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

    private void Inspect(ExtensionEntry ext)
    {
        ExtensionInspectTab eit = new ExtensionInspectTab(
            null, // previewUrl
            ext.iconUrl,
            ext.name,
            ext.version,
            null, // updatedDate
            ext.description,
            null // markdownUrl
        );
        Setting.AddTab(eit);
    }
}
