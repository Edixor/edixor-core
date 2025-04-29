using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FunctionLogicSettings", menuName = "Edixor/FunctionSettings", order = 1)]
public class FunctionSaveAsset : ScriptableObject
{
    [SerializeField]
    private List<FunctionData> functions = new List<FunctionData>();

    public List<FunctionData> SaveItems
    {
        get => functions;
        set => functions = value != null ? new List<FunctionData>(value) : new List<FunctionData>();
    }

}