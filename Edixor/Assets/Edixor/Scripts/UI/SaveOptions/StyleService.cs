using System.Collections.Generic;

public class StyleService : EdixorCurrentSetting<StyleSaveAsset, StyleData>
{
    public StyleService(IRegister register) : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorStyleSettings.asset"), register) { }

    public List<StyleData> GetStyles() => GetSettings().SaveItems;

    public void SetStyle(int index) => SetCurrentItem(index);
}
