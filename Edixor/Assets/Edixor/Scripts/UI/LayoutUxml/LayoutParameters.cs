using UnityEngine;


[CreateAssetMenu(fileName = "LayoutParameters", menuName = "Edixor/LayoutParameters")]
public class LayoutParameters : ScriptableObject
{
    [SerializeField] private LayoutElement[] elements;

    public LayoutElement[] Elements => elements;
}

[System.Serializable]
public class LayoutElement
{
    public string elementName;
    public string[] functionNames;
}