using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[OrderAttributeFactory(2)]
[Serializable]
public class IntertedDesign : EdixorDesign
{
    private VisualElement topSection;
    private VisualElement scrollView;
    private VisualElement leftSection;
    private VisualElement rightSection;
 
    private List<EdixorFunction> commandFunctions;
    private List<EdixorFunction> otherFunctions;

    public override string PathUxml => "Assets/Edixor/Scripts/UI/Design/Inverted/Inverted.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/Design/Inverted/Inverted.uss";
    public override string Image => "Assets/Edixor/Texture/EdixorWindow/Design/Standart1.png";

    [Header("Basic information")]
    [SerializeField]
    private string _desingName = "Interted";
    public string desingName
    {
        get { return _desingName; }
        private set { _desingName = value; }
    }

    public override string Name => "Interted";

    public override string Description => "added by developers in version: 00.00.01";

    public IntertedDesign(EdixorWindow window) : base(window) {
    }

    protected override void InitializeSections(VisualElement root)
    {
        topSection = root.Q<VisualElement>("tab-section");
        scrollView = root.Q<VisualElement>("middle-section-content");
        leftSection = root.Q<VisualElement>("function-left-section");
        rightSection = root.Q<VisualElement>("function-right-section");

        commandFunctions = SeparationоfFunctions(new[] { typeof(RestartFunction)});
        otherFunctions = SeparationоfFunctions(new[] { typeof(HotKeysFunction), typeof(SettingsFunction) });

        AddButtonsToSection(leftSection, commandFunctions);

        AddButtonsToSection(rightSection, otherFunctions);
    }
}
