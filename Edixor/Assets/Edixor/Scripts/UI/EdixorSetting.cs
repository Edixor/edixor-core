using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EdixorSettings", menuName = "Edixor/Settings", order = 1)]
public class EdixorSettings : ScriptableObject
{
    [SerializeReference] public List<EdixorDesign> designs;
    public int designIndex;
    public int designVersionIndex;
    
    [SerializeReference] public List<EdixorFunction> functions;

    [SerializeReference] public List<KeyAction> hotKeys;

    public bool isModified;

    private void OnEnable()
    {
        // Если окно открыто, устанавливаем его ссылку во все горячие клавиши
        EdixorWindow window = EditorWindow.GetWindow<EdixorWindow>();
        if (window != null && hotKeys != null)
        {
            foreach (var action in hotKeys)
            {
                action.SetWindow(window);
            }
        }
    }
}
