using UnityEngine;
using UnityEditor;
using ExTools;

[CreateAssetMenu(fileName = "Status", menuName = "Edixor/Status", order = 5)]
public class StatusData : ScriptableObject
{
    [SerializeField] private string statusName;
    [SerializeField] private string statusDescription;
    [SerializeField] private bool enable = true;
    public string Name => statusName;
    public string Description => statusDescription;
    public bool Enable => enable;
    
    [SerializeField]
    private MonoScript scriptLogic;
    public MonoScript ScriptLogic => scriptLogic;

    private void OnValidate()
    {
        if (scriptLogic == null)
            return;

        var type = scriptLogic.GetClass();

        if (type == null || !typeof(ExStatus).IsAssignableFrom(type))
        {
            Debug.LogError($"Script '{scriptLogic.name}' must inherit ExStatus. Clearing field.");
            scriptLogic = null;
        }
    }
}