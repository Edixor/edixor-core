using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BFPStyleComponent
{
    public string name;
    public TypeComponents type;
    public List<BFPStyleElementCollection> elementCollections;

    public GUIStyle GetElementGUI(TypeElements type, string name = null) {
        for (int i = 0; i < elementCollections.Count; i++)
        {
            if (elementCollections[i].type == type)
            {
                return elementCollections[i].GetElement(name).GetGUIStyle();
            }
        }

        return null;
    }
}
