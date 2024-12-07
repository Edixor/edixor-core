using UnityEditor;
using UnityEngine;

public class BottomSectionExW
{
    public void Draw(Texture2D backgroundTexture, EdixorWindow window)
    {
        GUILayout.BeginHorizontal(GUILayout.Height(25));
        GUI.DrawTexture(new Rect(0, window.position.height - 25, window.position.width, 25), backgroundTexture);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        
        if (GUI.Button(new Rect(5, window.position.height - 22, 20, 20), "<"))
        {
            Debug.Log("Кнопка 1 нажата");
        }
        
        if (GUI.Button(new Rect(30, window.position.height - 22, 20, 20), ">"))
        {
            Debug.Log("Кнопка 2 нажата");
        }

        if (GUI.Button(new Rect(60, window.position.height - 22, 20, 20), "[]"))
        {
            Debug.Log("Кнопка 3 нажата");
        }

        GUILayout.FlexibleSpace();

        // Вычисляем положение кнопки "S" для привязки к правому углу
        float buttonWidth = 20f;
        float buttonPadding = 5f; // Отступ от правого края
        float buttonPosX = window.position.width - buttonWidth - buttonPadding;

        if (GUI.Button(new Rect(buttonPosX, window.position.height - 22, buttonWidth, 20), "S"))
        {
            window.ChangeOfStyle();
        }

        GUILayout.EndHorizontal();
    }
}
