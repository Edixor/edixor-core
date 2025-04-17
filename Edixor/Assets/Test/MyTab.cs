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
        LoadHotKey("Assets/Test/KeyActionData.asset", OnButtonClick);
    }

    private void Start()
    {
        Button myButton = new Button(() => OnButtonClick())
        {
            text = "Нажми меня"
        };
        
        ParentContainer.Q<VisualElement>("root").Add(myButton);
    }

    private void OnButtonClick()
    {
        CustomMenu menu = ScriptableObject.CreateInstance<CustomMenu>();
        CustomMenu menu2 = ScriptableObject.CreateInstance<CustomMenu>();

        menu2.AddItem(new CMItemAction("Action 1", true, () => Debug.Log("Action 1 executed"), "action1"));
        menu2.AddItem(new CMItemAction("Action 2", true, () => Debug.Log("Action 2 executed"), "action2"));

        menu.AddItem(new CMItemAction("Action 1", true, () => Debug.Log("Action 1 executed"), "action1"));
        menu.AddItem(new CMItemBool("Action 2", true, false, "action2"));
        menu.AddItem(new CMItemMenu("Action 3", true, menu2, "action3"));

        menu.ShowMenu();
    }
}
