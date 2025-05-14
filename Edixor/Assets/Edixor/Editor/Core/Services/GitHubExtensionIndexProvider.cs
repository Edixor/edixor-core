using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking; 
using System.IO.Compression;
using UnityEngine; 
using System.IO;
using System;

public class GitHubExtensionIndexProvider : IExtensionIndexProvider
{
    private const string IndexUrl = "https://raw.githubusercontent.com/Terafy/edixor-extensions/main/Index.json";
    private const string RawBase  = "https://raw.githubusercontent.com/Terafy/edixor-extensions/main/";

    public async Task<List<IndexEntry>> LoadIndexAsync()
    {
        using var req = UnityWebRequest.Get(IndexUrl);
        var oper = req.SendWebRequest();
        while (!oper.isDone) await Task.Yield();
        if (req.result != UnityWebRequest.Result.Success)
            throw new Exception(req.error);

        string jsonText = req.downloadHandler.text.Trim();
        string processed = Regex.Replace(
            jsonText,
            "\\\"(.*?)\\\":\\s*\\{",
            m => "{\"name\":\"" + m.Groups[1].Value + "\",");
        processed = processed.TrimStart('{').TrimEnd('}');
        string wrapped = "{\"entries\":[" + processed + "]}";
        var root = JsonUtility.FromJson<IndexRoot>(wrapped);

        foreach (var e in root.entries)
        {
            e.jsonUrl = Normalize(RawBase, e.json);
            e.zipUrl  = Normalize(RawBase, e.zip);
            e.iconUrl = string.IsNullOrEmpty(e.icon) ? null : Normalize(RawBase, e.icon);
        }
        return root.entries;
    }

    private string Normalize(string baseUrl, string path)
        => path.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            ? path
            : baseUrl + path;
}