using UnityEditor;
using UnityEngine;

public class EdixorWindow : EditorWindow
{
    private Texture2D whiteTexture;
    private Texture2D blackTexture;

    private TopSectionExW topSection;
    private MiddleSectionExW middleSection;
    private BottomSectionExW bottomSection;

    private CustomMenu menu;

    private Rect rect;
    private bool isHidden = false;
    private bool isPositionModified = false;

    [MenuItem("Window/BFP test")]
    public static void ShowWindow()
    {
        GetWindow<EdixorWindow>("BFP test");
    }

    private void OnEnable()
    {
        // Создаем текстуры
        CreateTextures();

        // Инициализируем секции
        topSection = new TopSectionExW();
        middleSection = new MiddleSectionExW();
        bottomSection = new BottomSectionExW();
    }

    private void CreateTextures()
    {
        whiteTexture = new Texture2D(1, 1);
        whiteTexture.SetPixel(0, 0, new Color(0.6f, 0.6f, 0.6f, 1f));
        whiteTexture.Apply();

        blackTexture = new Texture2D(1, 1);
        blackTexture.SetPixel(0, 0, new Color(0.4f, 0.4f, 0.4f, 1f));
        blackTexture.Apply();
    }

    public void ChangeOfStyle()
    {
        if (menu == null)
        {
            menu = ScriptableObject.CreateInstance<CustomMenu>();
            menu.Initialize(null);
        }

        menu.AddItem(new CMItemAction("test1", true, () => {}, "ASD1"));
        menu.AddItem(new CMItemAction("test2", true, () => {}, "ASD2"));
        menu.AddItem(new CMItemAction("test3", true, () => {}, "ASD3"));

        Vector2 position = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);

        menu.ShowMenu(position);
    }


    private void OnGUI()
    {
        HandleWindowPosition();

        GUILayout.BeginVertical();

        DrawTopSection();

        middleSection.Draw(blackTexture, position.height, position.width);

        DrawBottomSection();

        GUILayout.EndVertical();
    }

    private void HandleWindowPosition()
    {
        if (isHidden)
        {
            if (!isPositionModified)
            {
                rect = new Rect(position.x, position.y, position.width, position.height);
                isPositionModified = true;
            }
            position = new Rect(position.x, position.y, 100, 100);
        }
        else if (isPositionModified)
        {
            position = rect;
            isPositionModified = false;
        }
    }

    private void DrawTopSection()
    {
        topSection.Draw(whiteTexture, this, ref isHidden);
    }

    private void DrawBottomSection()
    {
        bottomSection.Draw(whiteTexture, this);
    }

    private void OnDisable()
    {
        // Освобождаем ресурсы текстур
        if (whiteTexture != null)
        {
            DestroyImmediate(whiteTexture);
        }
        if (blackTexture != null)
        {
            DestroyImmediate(blackTexture);
        }
    }
}
