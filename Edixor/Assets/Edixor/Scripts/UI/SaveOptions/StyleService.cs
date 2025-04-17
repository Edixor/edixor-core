using System.Collections.Generic;

public class StyleService : EdixorCurrentSetting<StyleSaveAsset, StyleData>
{
    public StyleService(IRegister register) : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorStyleSettings.asset"), register) { }

    public List<StyleData> GetStyles() => GetSettings().SaveItems;

    public void SetStyle(int index) => SetCurrentItem(index);

    public StyleParameters GetStyleParameter(int styleIndex, int parameterIndex)
    {
        var styles = GetStyles();
        if (styleIndex < 0 || styleIndex >= styles.Count)
        {
            UnityEngine.Debug.LogError($"Style index {styleIndex} is out of range.");
            return null;
        }

        var styleData = styles[styleIndex];
        var parameters = styleData.GetAssetParameters();
        if (parameterIndex < 0 || parameterIndex >= parameters.Count)
        {
            UnityEngine.Debug.LogError($"Parameter index {parameterIndex} is out of range for style {styleData.Name}.");
            return null;
        }

        return parameters[parameterIndex];
    }
}