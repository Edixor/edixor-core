using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "EdixorStyle", menuName = "Edixor/Style/Data", order = 0)]
public class StyleData : ScriptableObject
{
    [SerializeField]
    private string StyleName;
    public string Name => StyleName;
    [SerializeField]
    private StyleParameters[] assetParameters = new StyleParameters[0];

    public StyleParameters[] AssetParameters => assetParameters;

    public EdixorParameters GetEdixorParameters()
    {
        for (int i = 0; i < assetParameters.Length; i++)
        {
            var edixor = assetParameters[i] as EdixorParameters;
            if (edixor != null)
                return edixor;
        }
        return null;
    }

    public EdixorParameters GetEdixorParametersByName(string name)
    {
        for (int i = 0; i < assetParameters.Length; i++)
        {
            var edixor = assetParameters[i] as EdixorParameters;
            if (edixor != null && edixor.name == name)
                return edixor;
        }
        return null;
    }
}
