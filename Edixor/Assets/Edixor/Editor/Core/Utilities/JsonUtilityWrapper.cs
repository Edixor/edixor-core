using System;
using System.Collections.Generic;
using UnityEngine;

public static class JsonUtilityWrapper
{
    [Serializable]
    private class StringArray { public List<string> items; }

    public static List<string> FromJsonList(string json)
    {
        var wrapped = "{\"items\":" + json + "}";
        return JsonUtility.FromJson<StringArray>(wrapped).items;
    }
}