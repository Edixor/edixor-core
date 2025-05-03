using UnityEngine;

[CreateAssetMenu(fileName = "Function", menuName = "Edixor/Function", order = 4)]
public class FunctionData : ScriptableObject
{
    [SerializeField] private string functionName;
    [SerializeField] private string functionDescription;
    [SerializeField] private Texture functionIcon;
    public string Name => functionName;
    public Texture Icon => functionIcon;
    public string Description => functionDescription;
    
    [SerializeField]
    private string logicKey;
    public string LogicKey
    {
        get => logicKey;
        private set => logicKey = value;
    }
}