using UnityEngine;
using System;

[Serializable]
public class ExtensionEntry
{
    public string name;
    public string description;
    public string version;
    public string stage;
    public string status;
    public string type;
    public string icon;
    public string preview;
    public string inspection;
    public string json;
    public string zip;

    [NonSerialized] public string jsonUrl;
    [NonSerialized] public string zipUrl;
    [NonSerialized] public string iconUrl;
    [NonSerialized] public string previewUrl;
    [NonSerialized] public string inspectionUrl;

    public void ResolveUrls(string basePath)
    {
        jsonUrl = $"{basePath}{json}";
        zipUrl = zip != null ? $"{basePath}{zip}" : null;
        iconUrl = icon != null ? $"{basePath}{icon}" : null;
        previewUrl = preview != null ? $"{basePath}{preview}" : null;
        inspectionUrl = inspection != null ? $"{basePath}{inspection}" : null;
    }
}
