using UnityEngine;
using System;

[Serializable]
public class IndexEntry
{
    public string name;
    public string description;
    public string version;
    public string json;
    public string zip;
    public string icon;

    [NonSerialized] public string jsonUrl;
    [NonSerialized] public string zipUrl;
    [NonSerialized] public string iconUrl;
}
