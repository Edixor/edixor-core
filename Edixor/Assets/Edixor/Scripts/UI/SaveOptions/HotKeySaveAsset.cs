using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "HotKeySettings", menuName = "Edixor/HotKeySettings", order = 1)]
public class HotKeySaveAsset : ScriptableObject, ISaveAsset<KeyActionData>
{
    [SerializeField]
    private List<KeyActionData> hotKeys = new List<KeyActionData>();

    public List<KeyActionData> SaveItems
    {
        get => new List<KeyActionData>(hotKeys); 
        set => hotKeys = value != null ? new List<KeyActionData>(value) : new List<KeyActionData>();
    }


}
