using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class SettingTab : EdixorTab
{
    private FunctionService functionSave;
    private StyleLogic styleLogic = new StyleLogic();
    private StyleParameters styleParameters;
    private StyleService styleService;

    [MenuItem("Window/Edixor Tab/Setting")]
    public static void ShowTab()
    {
        ShowTab<SettingTab>();
    }

    private void Awake()
    {
        tabName = "Setting";
        LoadUxml("Assets/Edixor/Scripts/UI/EdixorTab/SettingTab/SettingTab.uxml");
        LoadUss("Assets/Edixor/Scripts/UI/EdixorTab/SettingTab/SettingTab.uss");
    }

    private void Start()
    {
        functionSave = container.ResolveNamed<FunctionService>(ServiceNames.FunctionSetting);
        styleService = container.ResolveNamed<StyleService>(ServiceNames.StyleSetting);

        SetupContainer("layout-container", AddLayoutToContainer);
        SetupContainer("style-container", AddStyleToContainer);

        AddFunctionSettings(functionSave.GetFunctions());
    }

    private void SetupContainer(string containerName, Action<VisualElement> action)
    {
        VisualElement container = root.Q<VisualElement>(containerName);
        if (container == null)
        {
            Debug.LogError($"{containerName} not found.");
            return;
        }
        action(container);
    }

    private void AddLayoutToContainer(VisualElement designContainer)
    {
        var layoutDataArray = container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).GetLayouts().ToArray();
        var styleData = styleService.GetCurrentItem();

        if (layoutDataArray.Length == 0)
        {
            Debug.LogWarning("Layouts not found.");
            return;
        }

        foreach (var (data, index) in layoutDataArray.Select((data, i) => (data, i)))
        {
            designContainer.Add(CreateBanner(data, styleData, index, true));
        }
    }

    private void AddStyleToContainer(VisualElement styleContainer)
    {
        var styleDataArray = styleService.GetStyles().ToArray();
        var layoutData = container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).GetCurrentItem();

        if (styleDataArray.Length == 0)
        {
            Debug.LogWarning("Styles not found.");
            return;
        }

        ScrollView scrollView = new ScrollView(ScrollViewMode.Horizontal);
        scrollView.AddToClassList("banner-scroll-view");

        foreach (var (data, index) in styleDataArray.Select((data, i) => (data, i)))
        {
            scrollView.Add(CreateBanner(data, layoutData, index, false));
        }

        styleContainer.Add(scrollView);
    }

    private VisualElement CreateBanner(object data, object layoutData, int index, bool isLayout)
    {
        VisualElement bannerContainer = new VisualElement();
        bannerContainer.AddToClassList("banner-container");

        VisualElement banner = new VisualElement();
        banner.AddToClassList("banner");
        banner.AddToClassList("middle-section");

        EdixorDesign edixorDesign = new EdixorDesign(
            isLayout ? (StyleData)layoutData : (StyleData)data,
            isLayout ? (EdixorLayoutData)data : (EdixorLayoutData)layoutData,
            banner, container);
        edixorDesign.LoadUI(true);

        styleParameters = styleService.GetStyleParameter<EdixorParameters>(isLayout ? GetStyleIndex((StyleData)layoutData) : index);
        SetBannerStyle(banner, styleParameters);

        string bannerName = isLayout ? ((EdixorLayoutData)data).Name : ((StyleData)data).Name;
        Label bannerNameLabel = new Label(bannerName);
        bannerNameLabel.AddToClassList("banner-name");

        Button selectButton = new Button(() =>
        {
            Debug.Log($"Banner with {(isLayout ? "layout" : "style")} {bannerName} selected.");
            if (isLayout)
                ChangeLayout(index);
            else
                ChangeStyle(index);

            container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow).RestartWindow();
        })
        { text = "Select" };

        selectButton.AddToClassList("select-button");

        bannerContainer.Add(bannerNameLabel);
        bannerContainer.Add(banner);
        bannerContainer.Add(selectButton);

        return bannerContainer;
    }

    private void SetBannerStyle(VisualElement banner, StyleParameters parameters)
    {
        banner.style.borderTopColor = new StyleColor(parameters.Colors[0]);
        banner.style.borderBottomColor = new StyleColor(parameters.Colors[0]);
        banner.style.borderLeftColor = new StyleColor(parameters.Colors[0]);
        banner.style.borderRightColor = new StyleColor(parameters.Colors[0]);
        styleLogic.Init(banner, parameters);
    }

    private void AddFunctionSettings(List<Function> functions)
    {
        VisualElement functionContainer = root.Q<VisualElement>("function-container");
        VisualElement functionSettingContainer = root.Q<VisualElement>("function-setting-container");
        Label infoLabel = new Label("Click on a function to configure it");
        functionSettingContainer.Add(infoLabel);

        foreach (Function func in functions)
        {
            if (func.Logic is IFunctionSetting settingFunc)
            {
                Button function = new Button(() =>
                {
                    functionSettingContainer.Clear();
                    Label functionTitle = new Label(func.Data.Name);
                    functionTitle.AddToClassList("function-title");
                    functionSettingContainer.Add(functionTitle);

                    if (func.Logic.Empty())
                    {
                        func.Logic.SetContainer(container);
                        func.Logic.Init();
                    }
                    settingFunc.Setting(functionSettingContainer);
                });

                if (func.Data.Icon is Texture2D backgroundImage)
                {
                    function.style.backgroundImage = new StyleBackground(backgroundImage);
                }
                else
                {
                    Debug.LogError("No icon found for function: " + func.Data.Name);
                }

                function.AddToClassList("function");
                functionContainer.Add(function);
            }
        }
    }

    private int GetStyleIndex(StyleData style)
    {
        var styles = styleService.GetStyles();
        int index = styles.IndexOf(style);
        if (index == -1)
        {
            Debug.LogError("Style not found in StyleService.");
        }
        return index;
    }
}
