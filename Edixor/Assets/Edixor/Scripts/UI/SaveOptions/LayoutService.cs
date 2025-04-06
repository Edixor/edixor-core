using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class LayoutService : EdixorCurrentSetting<LayoutSaveAsset, EdixorLayoutData>
{
    public LayoutService(Register register) : base(PathResolver.ResolvePath("Assets/Edixor/Scripts/Settings/EdixorLayoutDataSettings.asset"),
    register) { }

    public List<EdixorLayoutData> GetLayouts() => settings.SaveItems;

    public int GetCurrentLayoutIndex() => settings.CurrentIndex;

    public void SetLayout(int index) => SetCurrentItem(index);
    
}
