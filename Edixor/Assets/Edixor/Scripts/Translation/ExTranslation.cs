using UnityEngine;

public class Translation
{
    private Language[] languages;

    public void TranslationGUIContent(GUIContent content)
    {
        if (content != null && !string.IsNullOrEmpty(content.text))
        {
            content.text = "переведено";
        }
    }
}