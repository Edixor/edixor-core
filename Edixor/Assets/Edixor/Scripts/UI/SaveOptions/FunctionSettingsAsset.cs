using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FunctionLogicSettings", menuName = "Edixor/FunctionSettings", order = 1)]
public class FunctionSaveAsset : ScriptableObject, ISaveAsset<FunctionData>
{
    [SerializeField]
    private List<FunctionData> functions = new List<FunctionData>();

    public List<FunctionData> SaveItems
    {
        get => new List<FunctionData>(functions); 
        set => functions = value != null ? new List<FunctionData>(value) : new List<FunctionData>();
    }


}