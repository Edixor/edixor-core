using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorTabSettings", menuName = "Edixor/TabSettings", order = 1)]
public class TabSaveAsset : ScriptableObject, ISaveCurrentAsset<EdixorTab>
{
    [SerializeField]
    private List<EdixorTab> tabs = new List<EdixorTab>();

    public List<EdixorTab> SaveItems
    {
        get => new List<EdixorTab>(tabs); 
        set => tabs = value != null ? new List<EdixorTab>(value) : new List<EdixorTab>();
    }

    [SerializeField]
    private int tabIndex = 0;

    public int CurrentIndex { 
        get => tabIndex;
        set => tabIndex = value;
    }


}