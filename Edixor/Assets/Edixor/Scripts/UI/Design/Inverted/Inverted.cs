using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class IntertedDesign : EdixorDesign
{
    private VisualElement topSection;
    private VisualElement scrollView;
    private VisualElement leftSection;
    private VisualElement rightSection;
 
    private List<EdixorFunction> leftFunctions;
    private List<EdixorFunction> rightFunctions;

    public override string PathUxml => "Assets/Edixor/Scripts/UI/Design/Inverted/Inverted.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/Design/Inverted/Inverted.uss";
    public override string Image => "Assets/Edixor/Texture/EdixorWindow/Design/Standart1.png";
    public override string Name => "Lateral";
    public override string Description => "added by developers in version: 00.00.01";

    public IntertedDesign(EdixorWindow window) : base(window) {
    }

    protected override void InitializeSections(VisualElement root)
    {
        topSection = root.Q<VisualElement>("tab-section");
        scrollView = root.Q<VisualElement>("middle-section-content");
        leftSection = root.Q<VisualElement>("function-left-section");
        rightSection = root.Q<VisualElement>("function-right-section");

        leftFunctions = new List<EdixorFunction> { functions[0] };
        rightFunctions = new List<EdixorFunction> { functions[1] };

        AddButtonsToSection(leftSection, leftFunctions);

        AddButtonsToSection(rightSection, rightFunctions);
    }

    private void AddButtonsToSection(VisualElement section, List<EdixorFunction> functions)
    {
        if (section == null || functions == null) return;

        foreach (var func in functions)
        {
            Button button = new Button(() => func.Activate());

            if (func.Icon != null)
            {
                Image icon = new Image { image = func.Icon };
                button.Add(icon);
            }

            section.Add(button);
        }
    }
}
