using UnityEditor;
using UnityEngine;

public class TopSectionExW
{
    public void Draw(Texture2D backgroundTexture, EdixorWindow window, ref bool isHidden)
    {
        GUILayout.BeginHorizontal(GUILayout.Height(25));
        GUI.DrawTexture(new Rect(0, 0, window.position.width, 30), backgroundTexture);
        
        GUILayout.FlexibleSpace();
        
        // Оборачиваем кнопку в вертикальный контейнер
        GUILayout.BeginVertical();
        GUILayout.Space(3); // Отступ в 3 пикселя
        if (GUILayout.Button(isHidden ? "^" : "-", GUILayout.Width(20)))
        {
            isHidden = !isHidden; // Переключаем значение isHidden
        }
        GUILayout.EndVertical();
        
        GUILayout.EndHorizontal();
    }
}