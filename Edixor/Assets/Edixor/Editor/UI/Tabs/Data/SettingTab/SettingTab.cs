using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using ExTools;

[Serializable]
public class SettingTab : EdixorTab
{
    private FunctionSetting _functionSave;
    private StyleSetting _styleSetting;
    private StyleLogic _styleLogic = new StyleLogic();

    [MenuItem("Edixor/Tabs/Setting")]
    public static void ShowTab() => ShowTab<SettingTab>();

    private void Awake()
    {
        tabName = "Setting";
        LoadUxml("auto");
        LoadUss("auto");
    }

    // Это метод жизненного цикла, который EdixorTab найдёт и вызовет автоматически
    private void OnEnable()
    {
        // Получаем сервисы
        _functionSave = container.ResolveNamed<FunctionSetting>(ServiceNames.FunctionSetting);
        _styleSetting = container.ResolveNamed<StyleSetting>(ServiceNames.StyleSetting);

        // Перестраиваем UI
        RebuildLayouts();
        RebuildStyles();
        //RebuildFunctionSettings();
    }

    private void RebuildLayouts()
    {
        var layoutContainer = root.Q<VisualElement>("layout-container");
        if (layoutContainer == null)
        {
            ExDebug.LogError("layout-container not found.");
            return;
        }
        layoutContainer.Clear();

        var layouts = container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting).GetAllItem();
        var currentStyle = _styleSetting.GetCorrectItem();
        if (layouts == null || layouts.Length == 0)
        {
            ExDebug.LogWarning("Layouts not found.");
            return;
        }

        foreach (var (data, idx) in layouts.Select((d, i) => (d, i)))
            layoutContainer.Add(CreateBanner(data, currentStyle, idx, true));
    }

    private void RebuildStyles()
    {
        var styleContainer = root.Q<VisualElement>("style-container");
        if (styleContainer == null)
        {
            ExDebug.LogError("style-container not found.");
            return;
        }
        styleContainer.Clear();

        var styles = _styleSetting.GetAllItem().ToArray();
        var currentLayout = container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting).GetCorrectItem();
        if (styles == null || styles.Length == 0)
        {
            ExDebug.LogWarning("Styles not found.");
            return;
        }

        var scroll = new ScrollView(ScrollViewMode.Horizontal);
        scroll.AddToClassList("banner-scroll-view");
        foreach (var (data, idx) in styles.Select((d, i) => (d, i)))
            scroll.Add(CreateBanner(data, currentLayout, idx, false));
        styleContainer.Add(scroll);
    }

    private void RebuildFunctionSettings()
    {
        var funcContainer   = root.Q<VisualElement>("function-container");
        var configContainer = root.Q<VisualElement>("function-setting-container");
        if (funcContainer == null || configContainer == null)
        {
            ExDebug.LogError("Function containers not found.");
            return;
        }
        funcContainer.Clear();
        configContainer.Clear();

        // Инструктивная надпись
        configContainer.Add(new Label("Click on a function to configure it"));

        // Кнопки для каждой функции
        foreach (var func in _functionSave.GetAllItemFull())
        {
            if (func.Logic is IFunctionSetting)
            {
                var btn = new Button(() => ShowFunctionConfig(func, configContainer)){};
                if (func.Data.Icon is Texture2D icon)
                    btn.style.backgroundImage = new StyleBackground(icon);
                btn.AddToClassList("function");
                funcContainer.Add(btn);
            }
        }
    }

    private void ShowFunctionConfig(Function func, VisualElement configContainer)
    {
        configContainer.Clear();
        var title = new Label(func.Data.Name);
        title.AddToClassList("function-title");
        configContainer.Add(title);

        if (func.Logic.Empty())
        {
            func.Logic.SetContainer(container);
            func.Logic.Init();
        }
        ((IFunctionSetting)func.Logic).Setting(configContainer);
    }

    private VisualElement CreateBanner(object dataObj, object otherObj, int index, bool isLayout)
    {
        var bannerContainer = new VisualElement();
        bannerContainer.AddToClassList("banner-container");

        var banner = new VisualElement();
        banner.AddToClassList("banner");
        banner.AddToClassList("middle-section");

        // Определяем, что за data и style/layout
        var styleData  = isLayout ? (StyleData)otherObj  : (StyleData)dataObj;
        var layoutData = isLayout ? (LayoutData)dataObj : (LayoutData)otherObj;

        // Рисуем дизайн
        var edixorDesign = new EdixorDesign(styleData, layoutData, banner, container);
        edixorDesign.LoadUI(true);

        // Стилизация бордюров
        var parameters = styleData.AssetParameters[0];
        banner.style.borderTopColor    = new StyleColor(parameters.BorderColors[0]);
        banner.style.borderBottomColor = new StyleColor(parameters.BorderColors[0]);
        banner.style.borderLeftColor   = new StyleColor(parameters.BorderColors[0]);
        banner.style.borderRightColor  = new StyleColor(parameters.BorderColors[0]);
        _styleLogic.Init(banner, parameters);

        // Имя баннера
        string name = isLayout ? ((LayoutData)dataObj).Name : ((StyleData)dataObj).Name;
        var nameLabel = new Label(name);
        nameLabel.style.color = parameters.Colors[0];
        nameLabel.AddToClassList("banner-name");

        // Кнопка Select
        var selectBtn = new Button(() =>
        {
            if (isLayout)
                container.ResolveNamed<LayoutSetting>(ServiceNames.LayoutSetting).UpdateIndex(index);
            else
                container.ResolveNamed<StyleSetting>(ServiceNames.StyleSetting).UpdateIndex(index);

            container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow).RestartWindow();
        }) { text = "Select" };
        selectBtn.AddToClassList("select-button");
        selectBtn.style.color = parameters.Colors[0];
        selectBtn.style.backgroundColor = parameters.BackgroundColors[0];
        selectBtn.style.borderTopColor    = parameters.BorderColors[0];
        selectBtn.style.borderBottomColor = parameters.BorderColors[0];
        selectBtn.style.borderLeftColor   = parameters.BorderColors[0];
        selectBtn.style.borderRightColor  = parameters.BorderColors[0];

        bannerContainer.Add(nameLabel);
        bannerContainer.Add(banner);
        bannerContainer.Add(selectBtn);

        return bannerContainer;
    }
}
