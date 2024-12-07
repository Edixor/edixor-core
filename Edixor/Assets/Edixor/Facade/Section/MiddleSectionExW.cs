using UnityEditor;
using UnityEngine;

public class MiddleSectionExW
{
    private Vector2 scrollPosition; // Переменная для отслеживания позиции прокрутки

    public void Draw(Texture2D backgroundTexture, float windowHeight, float windowWidth)
    {
        GUILayout.BeginVertical(GUILayout.Height(windowHeight), GUILayout.Width(windowWidth));
        
        float centralAreaHeight = windowHeight - 50; // 30 (верх) + 30 (низ)
        GUI.DrawTexture(new Rect(0, 25, windowWidth, centralAreaHeight), backgroundTexture);
        
        // Начинаем область прокрутки
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(centralAreaHeight), GUILayout.Width(windowWidth));
        
        // Помещаем объекты в область прокрутки
        for (int i = 0; i < 20; i++)
        {
            GUILayout.Label("test");
        }
        
        // Заканчиваем область прокрутки
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
    }
}
