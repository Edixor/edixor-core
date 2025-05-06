using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using ExTools; // Для доступа к EdixorObjectLocator

public class ExtensionManagerTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Extensions")]
    public static void ShowTab() => ShowTab<ExtensionManagerTab>();

    private const string IndexUrl = "https://raw.githubusercontent.com/Terafy/edixor-extensions/main/Index.json";

    // Список всех метаданных расширений
    private List<ExtensionInfo> _extensions    = new List<ExtensionInfo>();
    private HashSet<string>   _loadedNames     = new HashSet<string>();
    private ScrollView        _listContainer;

    private void Awake()
    {
        Option("Extensions", "auto", "auto");
    }

    private void Start()
    {
        _listContainer = root.Q<ScrollView>("extension-list");
        if (_listContainer == null)
            Debug.LogError("ExtensionManagerTab: ScrollView 'extension-list' not found in UXML.");
        LoadIndex();
    }

    private void LoadIndex()
    {
        // Сбрасываем данные и UI перед каждым новым запросом
        _extensions.Clear();
        _loadedNames.Clear();
        _listContainer?.Clear();

        Debug.Log("Loading Index.json from: " + IndexUrl);
        UnityWebRequest req = UnityWebRequest.Get(IndexUrl);
        req.SendWebRequest().completed += _ =>
        {
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Index load error: {req.error}");
                return;
            }

            Debug.Log("Index.json loaded: " + req.downloadHandler.text);
            var urls = JsonUtilityWrapper.FromJsonList(req.downloadHandler.text);
            foreach (var url in urls)
            {
                Debug.Log("Processing extension URL: " + url);
                LoadExtensionMeta(url);
            }
        };
    }

    private void LoadExtensionMeta(string url)
    {
        // Приводим ссылку к raw.githubusercontent.com формату
        string requestUrl = NormalizeToRawUrl(url);
        Debug.Log($"Normalized URL: {requestUrl}");

        UnityWebRequest req = UnityWebRequest.Get(requestUrl);
        req.SendWebRequest().completed += _ =>
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"Meta loaded from {requestUrl}: {req.downloadHandler.text}");
                var info = JsonUtility.FromJson<ExtensionInfo>(req.downloadHandler.text);

                // Отсекаем дубли по имени
                if (_loadedNames.Add(info.name))
                {
                    _extensions.Add(info);
                    RenderList();
                }
                else
                {
                    Debug.Log($"Skipping duplicate extension: {info.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Meta load failed for {requestUrl}: {req.error}");
            }
        };
    }

    private void RenderList()
    {
        if (_listContainer == null) return;
        _listContainer.Clear();

        foreach (var ext in _extensions)
        {
            var box = new VisualElement();
            box.AddToClassList("extension-box");

            var title = new Label(ext.name);
            title.AddToClassList("extension-title");
            box.Add(title);

            var desc = new Label(ext.description);
            desc.AddToClassList("extension-desc");
            box.Add(desc);

            // Проверяем, установлено ли расширение
            string virtualPath = $"Extensions/{ext.name}";
            string resolved    = EdixorObjectLocator.Resolve(virtualPath);
            bool installed     = !string.IsNullOrEmpty(resolved)
                                 && Directory.Exists(Path.Combine(Application.dataPath, resolved.Substring("Assets/".Length)));

            var installBtn = new Button(() => InstallExtension(ext))
            {
                text = installed ? "Installed" : "Install"
            };
            installBtn.SetEnabled(!installed);
            box.Add(installBtn);

            _listContainer.Add(box);
        }
    }

    private void InstallExtension(ExtensionInfo ext)
    {
        string zipUrl     = ext.zipUrl;
        string tempZip    = Path.Combine(Application.temporaryCachePath, ext.name + ".zip");
        string extractDir = Path.Combine(Application.temporaryCachePath, ext.name);

        Debug.Log($"Downloading ZIP from {zipUrl}");
        UnityWebRequest req = UnityWebRequest.Get(zipUrl);
        req.SendWebRequest().completed += _ =>
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(tempZip, req.downloadHandler.data);

                if (Directory.Exists(extractDir))
                    Directory.Delete(extractDir, true);

                ZipFile.ExtractToDirectory(tempZip, extractDir);

                // Определяем правильную корневую папку внутри extractDir
                string sourceDir = extractDir;
                var subdirs = Directory.GetDirectories(extractDir);
                if (subdirs.Length == 1)
                {
                    string onlyDir = subdirs[0];
                    if (Path.GetFileName(onlyDir).Equals(ext.name, StringComparison.OrdinalIgnoreCase))
                    {
                        // Если в корне нет файлов, берем вложенную
                        if (Directory.GetFiles(extractDir).Length == 0)
                            sourceDir = onlyDir;
                    }
                }

                Debug.Log($"Importing directory {sourceDir} to virtual path Extensions/{ext.name}");
                EdixorObjectLocator.ImportDirectory(sourceDir, $"Extensions/{ext.name}");

                File.Delete(tempZip);
                AssetDatabase.Refresh();
                RenderList();
            }
            else
            {
                Debug.LogError($"Download failed: {req.error}");
            }
        };
    }

    // Помогает превратить URL GitHub в raw.githubusercontent.com
    private string NormalizeToRawUrl(string url)
    {
        if (url.Contains("raw.githubusercontent.com"))
            return url;

        if (url.StartsWith("https://github.com/"))
        {
            var parts = url.Replace("https://github.com/", "").Split(new[] {'/'}, 4);
            if (parts.Length >= 4)
            {
                string user = parts[0], repo = parts[1], branch = parts[2], path = parts[3];
                if (path.StartsWith("blob/")) path = path.Substring(5);
                return $"https://raw.githubusercontent.com/{user}/{repo}/{branch}/{path}";
            }
        }

        return url;
    }

    // Парсер JSON-массивов через JsonUtility
    private static class JsonUtilityWrapper
    {
        [Serializable]
        private class StringArray { public List<string> items; }

        public static List<string> FromJsonList(string json)
        {
            var wrapped = "{\"items\":" + json + "}";
            return JsonUtility.FromJson<StringArray>(wrapped).items;
        }
    }

    [Serializable]
    public class ExtensionInfo
    {
        public string name;
        public string description;
        public string version;
        public string zipUrl;
    }
}
