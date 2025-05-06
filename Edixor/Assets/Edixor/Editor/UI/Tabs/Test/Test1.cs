using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using ExTools;

public class Test1 : EdixorTab
{
    [MenuItem("Edixor/Tabs/Test1")]
    public static void ShowTab() => ShowTab<Test1>();

    private VisualElement _buttonContainer;

    private void Awake()
    {
        tabName = "Test1";
        LoadUxml("Tabs/Default/Default.uxml");
    }

    public void Start()
    {
        _buttonContainer = root.Q<VisualElement>("root");

        AddLogButton("Info Log", () => ExDebug.Log("Info button clicked"));
        AddLogButton("Comment Log", () => ExDebug.LogComment("Comment button clicked"));
        AddLogButton("Warning Log", () => ExDebug.LogWarning("Warning button clicked"));
        AddLogButton("Error Log", () => ExDebug.LogError("Error button clicked"));

        AddGroupButton("Group Log", "UI Group", new string[] { "First in group", "Second in group", "Third in group" });
    }

    private void AddLogButton(string label, System.Action onClick)
    {
        var button = new Button(onClick)
        {
            text = label
        };
        button.AddToClassList("log-button");
        _buttonContainer.Add(button);
    }

    private void AddGroupButton(string label, string groupName, string[] messages)
    {
        var button = new Button(() =>
        {
            ExDebug.BeginGroup(groupName);
            foreach (var msg in messages)
                ExDebug.Log(msg);
            ExDebug.EndGroup();
        })
        {
            text = label
        };
        button.AddToClassList("group-button");
        _buttonContainer.Add(button);
    }
}
