using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class GenericIndexProvider<T> : IGenericIndexProvider<T> where T : class
{
    private string _indexUrl;
    private const string VersionKey = "news_index_version";

    public void Init(string indexUrl)
    {
        _indexUrl = indexUrl;
        Debug.Log($"[GenericIndexProvider<{typeof(T).Name}>] Initialized with URL: {_indexUrl}");
    }

    public async Task<List<T>> LoadIndexAsync()
    {
        Debug.Log($"[GenericIndexProvider<{typeof(T).Name}>] Starting download...");
        using var req = UnityWebRequest.Get(_indexUrl);
        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result != UnityWebRequest.Result.Success)
            throw new Exception(req.error);

        string json = req.downloadHandler.text.Trim();
        Debug.Log($"[GenericIndexProvider<{typeof(T).Name}>] Raw JSON:\n{json}");
        var versionMatch = Regex.Match(json, "\"version\"\\s*:\\s*\"(.*?)\"", RegexOptions.Singleline);
        if (versionMatch.Success)
        {
            var newVersion = versionMatch.Groups[1].Value;
            var oldVersion = PlayerPrefs.GetString(VersionKey, "");
            if (newVersion != oldVersion)
            {
                Debug.Log($"[GenericIndexProvider] New index version {newVersion} (old {oldVersion})");
                PlayerPrefs.SetString(VersionKey, newVersion);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log($"[GenericIndexProvider] Index version unchanged: {oldVersion}");
            }
        }
        if (json.StartsWith("{") && json.Contains("\"entries\": {"))
        {
            Debug.Log("[GenericIndexProvider] Detected dictionary format, converting to array...");
            string arrayJson = ConvertDictEntriesToArray(json);
            Debug.Log($"[GenericIndexProvider] Converted JSON:\n{arrayJson}");
            return ParseArrayFormat(arrayJson);
        }
        if ((json.StartsWith("{") && json.Contains("\"entries\": ["))
         || json.StartsWith("["))
        {
            string arrayJson = json.StartsWith("[")
                ? "{\"entries\":" + json + "}"
                : json;
            Debug.Log("[GenericIndexProvider] Detected array format");
            return ParseArrayFormat(arrayJson);
        }

        throw new Exception("Unsupported JSON format: missing entries");
    }

    private List<T> ParseArrayFormat(string jsonWithArray)
    {
        try
        {
            var wrapper = JsonUtility.FromJson<Wrapper>(jsonWithArray);
            int count = wrapper.entries?.Count ?? 0;
            Debug.Log($"[GenericIndexProvider<{typeof(T).Name}>] Parsed entries count: {count}");
            return wrapper.entries ?? new List<T>();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[GenericIndexProvider<{typeof(T).Name}>] JSON parse error: {ex.Message}");
            return new List<T>();
        }
    }

    private string ConvertDictEntriesToArray(string originalJson)
    {
        var match = Regex.Match(originalJson, "\"entries\"\\s*:\\s*\\{(.*)\\}\\s*}\\s*$", RegexOptions.Singleline);
        if (!match.Success)
            throw new Exception("Cannot extract entries object");
        string inner = match.Groups[1].Value.Trim();
        string pattern = "\"(.*?)\"\\s*:\\s*\\{";
        string processed = Regex.Replace(inner, pattern,
            m => "{\"id\":\"" + m.Groups[1].Value + "\",");
        processed = processed.Trim().TrimEnd(',');
        return "{\"entries\":[" + processed + "]}";
    }

    [Serializable]
    private class Wrapper
    {
        public List<T> entries;
    }
}
