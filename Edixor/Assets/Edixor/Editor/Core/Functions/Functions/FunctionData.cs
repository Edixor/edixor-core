using UnityEngine;
using UnityEditor;
using ExTools;

[CreateAssetMenu(fileName = "Function", menuName = "Edixor/Function", order = 4)]
public class FunctionData : ScriptableObject
{
    [SerializeField] private string functionName;
    [SerializeField] private string functionDescription;
    [SerializeField] private Texture functionIcon;
    [SerializeField] private bool enable = true;
    public string Name => functionName;
    public Texture Icon => functionIcon;
    public string Description => functionDescription;
    public bool Enable => enable;
    
    [SerializeField]
    private MonoScript scriptLogic;
    public MonoScript ScriptLogic => scriptLogic;

    private void OnValidate()
    {
        if (scriptLogic == null)
            return;

        var type = scriptLogic.GetClass();

        if (type == null || !typeof(ExFunction).IsAssignableFrom(type))
        {
            Debug.LogError($"Script '{scriptLogic.name}' must inherit FunctionData. Clearing field.");
            scriptLogic = null;
        }
    }
}