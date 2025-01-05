using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements; 
using System.Linq;

public class DataStyle
{
    private readonly Dictionary<string, StyleSheet> styleCache = new();

    public void NewStyle(StyleSheet style, string path = null)
    {
        if (style == null) 
        {
            Debug.LogError("Стиль не может быть null.");
            return;
        }

        path ??= $"Assets/Styles/{style.name}.uss";
        AddStyle(path, style);
    }

    private void AddStyle(string path, StyleSheet style)
    {
        if (!styleCache.ContainsKey(path))
            styleCache[path] = style;
        else
            Debug.LogWarning($"Стиль по пути {path} уже добавлен.");
    }

    public StyleSheet GetStyle(string identifier) 
    {
        if (styleCache.TryGetValue(identifier, out var style))
            return style;

        Debug.LogError($"Стиль по пути {identifier} не найден.");
        return null;
    }

    public StyleSheet GetStyleByIndex(int index) 
        => IsIndexValid(index) ? styleCache.Values.ElementAt(index) : null;

    public string GetPath(string identifier) 
    {
        if (styleCache.ContainsKey(identifier))
            return identifier;

        Debug.LogError($"Путь по идентификатору {identifier} не найден.");
        return null;
    }

    public IEnumerable<string> GetAllPaths() => styleCache.Keys;

    public void Clear() => styleCache.Clear();

    private bool IsIndexValid(int index) 
    {
        if (index < 0 || index >= styleCache.Count) 
        {
            Debug.LogError($"Индекс {index} вне диапазона.");
            return false;
        }
        return true;
    }
}
