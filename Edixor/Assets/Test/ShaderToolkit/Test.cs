using UnityEngine;
using UnityEditor;

public class CustomEditorWindow : EditorWindow
{
    private Rect labelRect;
    private Texture2D texture;
    private Material material;
    private float time;

    [MenuItem("Window/Custom shader")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditorWindow>("Custom Ediw");
    }

    private void OnEnable()
    {
        // Инициализация текстуры
        texture = new Texture2D(256, 256);
        Color[] pixels = new Color[256 * 256];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.red; // Заполним текстуру красным цветом
        }
        texture.SetPixels(pixels);
        texture.Apply();

        // Поиск шейдера и создание материала
        Shader shader = Shader.Find("Custom/FlowingColors");
        material = new Material(shader);

        // Задаем начальные параметры для шейдера
        material.SetTexture("_MainTex", texture);
        material.SetFloat("_CustomTime", 0);
        material.SetColor("_Color1", Color.red);
        material.SetColor("_Color2", Color.yellow);
        material.SetFloat("_Speed", 1f);

        // Сброс ключевых слов шейдера (если они есть)
        material.shaderKeywords = null;

        // Регистрация обновления
        EditorApplication.update += UpdateShader;
    }

    private void OnDisable()
    {
        // Отмена регистрации обновления
        EditorApplication.update -= UpdateShader;
    }

    private void OnGUI()
    {
        // Рисуем текстуру с использованием шейдера
        if (material != null)
        {
            Rect rect = new Rect(10, 10, position.width - 20, position.height - 20);
            EditorGUI.DrawRect(rect, Color.black);

            // Рисуем текстуру с использованием Graphics.DrawTexture
            Graphics.DrawTexture(rect, texture, material);
        }
    }

    private void UpdateShader()
    {
        // Обновление времени и параметров шейдера
        time += Time.deltaTime;
        if (material != null)
        {
            material.SetFloat("_CustomTime", time);
            Repaint(); // Перерисовываем окно
        }
    }
}
