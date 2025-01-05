using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class NewTab : EdixorTab
{
    public NewTab(VisualElement ParentContainer) : base(ParentContainer) {}

    public override string Title => "New Tab";
    public override string PathUxml => "Assets/Edixor/Scripts/UI/EdixorTab/NewTab/NewTab.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/EdixorTab/NewTab/NewTab.uss";

    public override void OnUI() {
        //...
    }

}
