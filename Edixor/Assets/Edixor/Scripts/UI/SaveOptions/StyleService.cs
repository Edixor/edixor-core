using System.Collections.Generic;
using UnityEngine;

public class StyleService : EdixorCurrentSetting<StyleSaveAsset, StyleData>
{
    public StyleService(IRegister register)
        : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorStyleSettings.asset"), register)
    { }

    public List<StyleData> GetStyles() => GetSettings().SaveItems;

    public void SetStyle(int index) => SetCurrentItem(index);

    public T GetStyleParameter<T>(int? styleIndex = null) where T : StyleParameters
    {
        int actualStyleIndex = styleIndex ?? GetSettings().CurrentIndex;

        if (!IsValidIndex(actualStyleIndex, GetStyles().Count))
        {
            Debug.LogError($"Style index {actualStyleIndex} is out of range.");
            return null;
        }

        var styleData = GetStyles()[actualStyleIndex];
        foreach (var param in styleData.AssetParameters)
        {
            if (param is T typedParam)
                return typedParam;
        }

        Debug.LogError($"No parameter of type {typeof(T).Name} found in style '{styleData.Name}'.");
        return null;
    }

    private bool IsValidIndex(int index, int count)
    {
        return index >= 0 && index < count;
    }
}
