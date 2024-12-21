using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Style", menuName = "Edixor/Style", order = 1)]
public class ExUITModel : ScriptableObject {
    public string styleName;
    public ExUITComponent[] components;
}

