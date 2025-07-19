using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

using ExTools.Tabs.Advanced;
using ExTools.Settings;
using ExTools;

namespace ExTools.Tabs.Basic 
{
    public class NewTab : EdixorTab
    {
        [MenuItem("Edixor/Tabs/New Tab")]
        public static void ShowTab() => ShowTab<NewTab>();

        private IGenericIndexProvider<NewsEntry> _newsProvider;
        private List<NewsEntry> _allNewsEntries = new List<NewsEntry>();
        private VisualElement _newsListContainer;
        private Button _loadMoreButton;
        private int _displayCount = 3;

        private static readonly Dictionary<Type, string> menuPaths = new()
        {
            { typeof(NewTab), "Edixor/Tabs/New Tab" }
        };
        private static readonly Dictionary<Type, string> _menuPathCache = new();
        private static readonly HashSet<string> validTabPaths = new(StringComparer.OrdinalIgnoreCase);

        private void Awake()
        {
            Option("New Tab", "auto", "auto", "Resources/Images/Icons/edix.png");
        }

        private void Start()
        {
            Label versionLabel = root.Q<Label>("version-label");
            if (versionLabel != null)
            {
                string version = container
                    .ResolveNamed<OtherEdixorSetting>(ServiceNameKeys.OtherEdixorSetting)
                    .GetEdixorVersion();
                versionLabel.text = $"Version: {version}";
            }

            Label updatesLabel = root.Q<Label>("updates-label");
            if (updatesLabel != null)
                updatesLabel.text = $"Recent updates: {DateTime.Now:dd.MM.yyyy}";

            Label versionUnityLabel = root.Q<Label>("version-unity-label");
            if (versionUnityLabel != null)
            {
                string unityVersion = container
                    .ResolveNamed<OtherEdixorSetting>(ServiceNameKeys.OtherEdixorSetting)
                    .GetSupportedUnityVersions();
                versionUnityLabel.text = $"Unity Version: {unityVersion}";
            }

            Label sizeLabel = root.Q<Label>("size-label");
            if (sizeLabel != null)
            {
                string folderPath = EdixorObjectLocator.GetEdixorRootFolder();
                string formattedSize = FolderSizeCalculator.GetFolderSizeFormatted(folderPath);
                sizeLabel.text = $"Size: {formattedSize}";
            }

            InitValidTabPathsFromTabs();
            SetupLinksSection();
            SetupTabsSection();
            SetupNewsSection();
        }

        private void InitValidTabPathsFromTabs()
        {
            var allTabs = EdixorObjectLocator
                .FindAndCreateInstances<MonoScript, EdixorTab>("Tabs");

            foreach (var tab in allTabs)
            {
                string path = GetMenuItemPathForTab(tab.GetType(), tab.Title);
                if (!string.IsNullOrEmpty(path))
                    validTabPaths.Add(path);
            }
        }

        private void SetupLinksSection()
        {
            var linksSection = root.Q<VisualElement>("links-section");
            if (linksSection == null) return;
            linksSection.Clear();

            var docSection = new VisualElement();
            docSection.AddToClassList("section");
            var docHeader = new Label("Documentation:");
            docHeader.AddToClassList("section-header");
            docSection.Add(docHeader);

            var docButtons = new VisualElement();
            docButtons.AddToClassList("buttons-container");
            var docs = new (string, string)[]
            {
                ("GitBook Documentation", "https://www.notion.so/"),
                ("GitHub Documentation", "https://github.com/Edixor/edixor-documentation"),
            };
            foreach (var (text, url) in docs)
            {
                var btn = new Button(() => OpenUrl(url)) { text = text };
                btn.AddToClassList("E-button");
                docButtons.Add(btn);
            }
            docSection.Add(docButtons);

            var gitSection = new VisualElement();
            gitSection.AddToClassList("section");
            var gitHeader = new Label("GitHub:");
            gitHeader.AddToClassList("section-header");
            gitSection.Add(gitHeader);

            var gitButtons = new VisualElement();
            gitButtons.AddToClassList("buttons-container");
            var gitLinks = new (string, string)[]
            {
                ("GitHub Edixor", "https://github.com/Edixor/edixor-core"),
                ("GitHub Edixor-Extensions", "https://github.com/Edixor/edixor-extensions"),
                ("GitHub Edixor-News", "https://github.com/Edixor/edixor-news")
            };
            foreach (var (text, url) in gitLinks)
            {
                var btn = new Button(() => OpenUrl(url)) { text = text };
                btn.AddToClassList("E-button");
                gitButtons.Add(btn);
            }
            gitSection.Add(gitButtons);

            var divider = new VisualElement();
            divider.AddToClassList("divider");

            linksSection.Add(docSection);
            linksSection.Add(divider);
            linksSection.Add(gitSection);
        }

        private void SetupTabsSection()
        {
            var tabsList = root.Q<VisualElement>("tabs-list");
            if (tabsList == null)
                return;

            tabsList.Clear();

            var allTabs = EdixorObjectLocator
                .FindAndCreateInstances<MonoScript, EdixorTab>("Tabs")
                .Where(tab => tab.GetType() != typeof(NewTab))
                .ToList();

            VisualElement searchBar = UIBuilder.CreateSearchBar(
                allTabs,
                tab => tab.Title,
                filtered => DisplayFilteredTabs(tabsList, filtered),
                "Enter tab title",
                new[]
                {
                    new SearchExtraAction(
                        "Resources/Images/Icons/sett.png",
                        query =>
                        {
                            if (string.IsNullOrWhiteSpace(query))
                                return false;

                            string trimmedQuery = query.Trim();

                            if (!trimmedQuery.StartsWith("Edixor/Tabs/", StringComparison.OrdinalIgnoreCase))
                                return false;

                            bool found = validTabPaths.Contains(trimmedQuery);

                            if (!found)
                                return false;

                            return EditorApplication.ExecuteMenuItem(trimmedQuery);
                        },
                        "Open tab by path"
                    )
                }
            );

            tabsList.Add(searchBar);
            DisplayFilteredTabs(tabsList, allTabs);
        }

        private void DisplayFilteredTabs(VisualElement tabsList, List<EdixorTab> tabs)
        {
            var searchBar = tabsList.Q<VisualElement>("search-bar");
            tabsList.Clear();
            if (searchBar != null)
                tabsList.Add(searchBar);

            var factory = new RecommendedTabsUIFactory();
            foreach (var tab in tabs)
            {
                tab.Initialize(new VisualElement(), container);
                tab.InvokeAwake();
                tab.EnsureTabOpensLoaded();

                var path = GetMenuItemPathForTab(tab.GetType(), tab.Title);
                if (string.IsNullOrEmpty(path))
                    continue;

                var element = factory.CreateRecommendedTabElement(
                    tab.GetMaxIcon(),
                    tab.Title,
                    tab.OpenCount,
                    path
                );
                tabsList.Add(element);
            }
        }

        private string GetMenuItemPathForTab(Type type, string title)
        {
            if (menuPaths.TryGetValue(type, out var p)) return p;
            if (_menuPathCache.TryGetValue(type, out var cp)) return cp;

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                var attr = method.GetCustomAttribute<MenuItem>();
                if (attr != null)
                {
                    _menuPathCache[type] = attr.menuItem;
                    return attr.menuItem;
                }
            }

            string fallback = "Edixor/Tabs/" + title;
            _menuPathCache[type] = fallback;
            return fallback;
        }

        private void SetupNewsSection()
        {
            var newsContainer = root.Q<VisualElement>("news-container");
            if (newsContainer == null) return;
            newsContainer.Clear();

            var header = new Label("News");
            header.AddToClassList("section-header");
            newsContainer.Add(header);

            var searchBar = UIBuilder.CreateSearchBar(
                _allNewsEntries,
                news => news.title,
                filtered =>
                {
                    _displayCount = 3;
                    DisplayNews(filtered);
                },
                "Enter news title"
            );
            newsContainer.Add(searchBar);

            _newsListContainer = new VisualElement();
            newsContainer.Add(_newsListContainer);

            LoadNewsAsync();
        }

        private async void LoadNewsAsync()
        {
            _displayCount = 3;
            var factory = container.Resolve<IExtensionIndexProviderFactory>();
            _newsProvider = factory.Create<NewsEntry>(
                "https://raw.githubusercontent.com/Terafy/edixor-news/main/index.json"
            );
            try { _allNewsEntries = await _newsProvider.LoadIndexAsync(); }
            catch { _allNewsEntries = new List<NewsEntry>(); }

            if (_allNewsEntries == null || _allNewsEntries.Count == 0)
            {
                if (_newsListContainer != null)
                {
                    _newsListContainer.Clear();
                    var empty = new Label("No news available yet.");
                    empty.AddToClassList("empty-label");
                    _newsListContainer.Add(empty);
                }
                return;
            }
            DisplayNews(_allNewsEntries);
        }

        private async void DisplayNews(List<NewsEntry> list)
        {
            Debug.Log($"[News] Loaded: {_allNewsEntries.Count}");
            if (_newsListContainer == null)
                return;

            _newsListContainer.Clear();

            if (list == null || list.Count == 0)
            {
                var empty = new Label("No news found.");
                empty.AddToClassList("empty-label");
                _newsListContainer.Add(empty);
            }
            else
            {
                int count = Math.Min(_displayCount, list.Count);
                for (int i = 0; i < count; i++)
                {
                    var news = list[i];
                    var req = UnityWebRequestTexture.GetTexture(news.preview);
                    var op = req.SendWebRequest();
                    while (!op.isDone) await Task.Yield();

                    Texture2D tex = req.result == UnityWebRequest.Result.Success
                        ? DownloadHandlerTexture.GetContent(req)
                        : null;

                    var item = NewsUIFactory.CreateNewsElement(news, tex, entry =>
                    {
                        Setting.AddTab(new NewsTab(entry.path, entry.title, entry.date, entry.preview));
                    });

                    _newsListContainer.Add(item);
                }

                if (list.Count > _displayCount)
                {
                    if (_loadMoreButton == null)
                    {
                        _loadMoreButton = new Button(() =>
                        {
                            _displayCount = list.Count;
                            DisplayNews(list);
                        })
                        { text = "Load More" };
                        _loadMoreButton.AddToClassList("E-button");
                    }

                    if (!_newsListContainer.Contains(_loadMoreButton))
                        _newsListContainer.Add(_loadMoreButton);
                }
            }

            var parameters = (EdixorParameters)container
                .ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting)
                .GetCorrectItem()
                .GetEdixorParameters();
            InitStyleTab(parameters);
        }

        private void OpenUrl(string url) => Application.OpenURL(url);
    }
}
