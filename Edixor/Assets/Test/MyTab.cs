using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

public class MyTab : EdixorTab
{
    [MenuItem("Window/Edixor Tab/Custom Tab")]
    public static void ShowTab()
    {
        ShowTab<MyTab>();
    }

    private void Awake()
    {
        tabName = "My Custom Tab";
        LoadHotKey("Assets/Test/KeyActionData.asset", GenericMenuDemonstration);
    }

    private void Start()
    {
        Button buttonCustomMenu = new Button(() => MenuDemonstration())
        {
            text = "Custom Menu"
        };
        root.Q<VisualElement>("root").Add(buttonCustomMenu);

        Button buttonGenericMenu = new Button(() => GenericMenuDemonstration())
        {
            text = "Generic Menu"
        };
        root.Q<VisualElement>("root").Add(buttonGenericMenu);
    }

    private void MenuDemonstration()
    {
        CustomMenu menu = ScriptableObject.CreateInstance<CustomMenu>();

        menu.AddItem(new CMItemAction("Option 1", true, () => Debug.Log("Option 1 selected")));
        menu.AddItem(new CMItemAction("Option 2", true, () => Debug.Log("Option 2 selected")));
        menu.AddItem(new CMItemAction("Option 3", true, () => Debug.Log("Option 3 selected")));

        menu.AddStyle(container.ResolveNamed<StyleService>(ServiceNames.StyleSetting).GetStyleParameter<MenuParameters>());

        menu.ShowMenu();
    }

    private void GenericMenuDemonstration()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Option 1"), false, () => Debug.Log("Option 1 selected"));
        menu.AddItem(new GUIContent("Option 2"), false, () => Debug.Log("Option 2 selected"));
        menu.AddItem(new GUIContent("Option 3"), false, () => Debug.Log("Option 3 selected"));

        menu.ShowAsContext();
    }
}
