using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

[OrderAttributeFactory(1)]
[Serializable]
public class StandardDesign : EdixorDesign, IVersions
{
    private VisualElement tabSection, commandSection, otherSection, functionSection;
    private List<EdixorFunction> commandFunctions, otherFunctions;
    public Dictionary<int, Action> versions { get; private set; }
    public int countVersion { get; private set; }

    public override string PathUxml => "Assets/Edixor/Scripts/UI/Design/Standart/EdixorWIndow.uxml";
    public override string PathUss => "Assets/Edixor/Scripts/UI/Design/Standart/EdixorWIndow.uss";
    public override string Image => "Assets/Edixor/Texture/EdixorWindow/Design/Standart1.png";
    [Header("Basic information")]
    [SerializeField]
    private string _desingName = "Standard";
    public string desingName
    {
        get { return _desingName; }
        private set { _desingName = value; }
    }

    public override string Name => "Standard";
    public override string Description => "added by developers in version: 00.00.01";

    public StandardDesign(EdixorWindow window) : base(window) { }

    public void InitializeVersionActions()
    {
        versions = new Dictionary<int, Action>
        {
            { 0, () => Debug.Log("Version 0 logic") },
            { 1, SwapCommandAndOtherSections },
            { 2, SwapTabAndFunctionSections },
            { 3, () => { SwapCommandAndOtherSections(); SwapTabAndFunctionSections(); } }
        };

        countVersion = 4;
    }

    protected override void InitializeSections(VisualElement root)
    {
        tabSection = root.Q<VisualElement>("tab-section");
        commandSection = root.Q<VisualElement>("command-function-section");
        otherSection = root.Q<VisualElement>("other-function-section");
        functionSection = root.Q<VisualElement>("function-section");

        if (commandSection == null || otherSection == null)
        {
            Debug.LogError("Failed to initialize sections.");
            return;
        }

        commandFunctions = SeparationоfFunctions(new[] { typeof(RestartFunction), typeof(CloseFunction) });
        otherFunctions = SeparationоfFunctions(new[] { typeof(HotKeysFunction), typeof(SettingsFunction) });

        AddButtonsToSection(commandSection, commandFunctions);
        AddButtonsToSection(otherSection, otherFunctions);

        InitializeVersionActions();
        ChangeVersion(window.GetSetting().GetDesignVersion());
    }

    public void ChangeVersion(int version)
    {
        if (versions == null) InitializeVersionActions();

        if (versions.TryGetValue(version, out var action))
        {
            action.Invoke();
        }
        else
        {
            Debug.LogWarning($"Version {version} not implemented.");
        }
    }

    private void SwapCommandAndOtherSections()
    {
        SwapSections(commandSection, otherSection, false);
    }

    private void SwapTabAndFunctionSections()
    {
        SwapSections(tabSection, functionSection, true);
    }
}