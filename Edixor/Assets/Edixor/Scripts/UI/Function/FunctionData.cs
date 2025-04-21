using UnityEngine;

[CreateAssetMenu(fileName = "Function", menuName = "Edixor/Function", order = 4)]
public class FunctionData : ScriptableObject
{
    [SerializeField] private string functionName;
    [SerializeField] private string functionDescription;
    [SerializeField] private Texture functionIcon;
    [TextArea][SerializeField] private string functionIconPath;
    [TextArea][SerializeField] private string FunctionLogic;

    public string Name => functionName;
    public Texture Icon
    {
        get
        {
            if (functionIcon == null && !string.IsNullOrEmpty(functionIconPath))
            {
                functionIcon = Resources.Load<Texture>(functionIconPath);
                if (functionIcon == null)
                {
                    Debug.LogWarning($"Не удалось загрузить иконку по пути: {functionIconPath}");
                }
            }
            return functionIcon;
        }
    }
    public string Description => functionDescription;
    public string Logic => FunctionLogic;
    
    [SerializeField]
    private string logicKey;
    public string LogicKey
    {
        get => logicKey;
        private set => logicKey = value;
    }
}