using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class SettingTab : EdixorTab
{
    private EdixorUIManager edixorUIManager;
    private FunctionService functionSave;
    private IFactory factoryBuilder;
    private StyleLogic styleLogic = new StyleLogic();
    private StyleParameters styleParameters;

    public SettingTab(VisualElement ParentContainer, DIContainer container)
    {
        Init(container);
    }

    public void Init(DIContainer container = null, VisualElement containerUI = null) {
        functionSave = container.ResolveNamed<FunctionService>(ServiceNames.FunctionSetting);
        factoryBuilder = container.Resolve<IFactory>();
    }

    protected void OnTabUI()
    {
        SetupContainer("layout-container", AddLayoutToContainer);
        SetupContainer("style-container", AddStyleToContainer);

        factoryBuilder.Init<FunctionData, FunctionLogica, Function>(data => data.Logica);
        AddFunctionSettings(factoryBuilder.CreateAllFromProject().OfType<Function>().ToList());
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
        var layoutDatas = container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).GetLayouts().ToArray();
        var styleData = container.ResolveNamed<StyleService>(ServiceNames.StyleSetting).GetCurrentItem();
        
        if (layoutDatas.Length == 0)
        {
            Debug.LogWarning("Layouts not found.");
            return;
        }

        foreach (var (data, index) in layoutDatas.Select((data, i) => (data, i)))
        {
            designContainer.Add(CreateBanner(data, styleData, index, true));
        }
    }

    private void AddStyleToContainer(VisualElement styleContainer)
    {
        var styleDatas = container.ResolveNamed<StyleService>(ServiceNames.StyleSetting).GetStyles().ToArray();
        var layoutData = container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).GetCurrentItem();

        if (styleDatas.Length == 0)
        {
            Debug.LogWarning("Styles not found.");
            return;
        }

        ScrollView scrollView = new ScrollView(ScrollViewMode.Horizontal);
        scrollView.AddToClassList("banner-scroll-view");

        foreach (var (data, index) in styleDatas.Select((data, i) => (data, i)))
        {
            scrollView.Add(CreateBanner(data, layoutData, index, false));
        }

        styleContainer.Add(scrollView);
    }

    private VisualElement CreateBanner(object data, object layoutData, int index, bool isLayout)
    {
        // Создаём основной контейнер для баннера
        VisualElement bannerContainer = new VisualElement();
        bannerContainer.AddToClassList("banner-container");

        // Создаём элемент баннера и настраиваем его
        VisualElement banner = new VisualElement();
        banner.AddToClassList("banner");
        banner.AddToClassList("middle-section");

        // Инициализируем дизайн баннера
        EdixorDesign edixorDesign = new EdixorDesign(
            isLayout ? (StyleData)layoutData : (StyleData)data, 
            isLayout ? (EdixorLayoutData)data : (EdixorLayoutData)layoutData, 
            banner, container);
        edixorDesign.LoadUI(true);

        styleParameters = isLayout ? ((StyleData)layoutData).GetAssetParameter() : ((StyleData)data).GetAssetParameter();
        SetBannerStyle(banner, styleParameters);

        // Получаем имя баннера и создаём Label для его отображения сверху
        string bannerName = isLayout ? ((EdixorLayoutData)data).Name : ((StyleData)data).Name;
        Label bannerNameLabel = new Label(bannerName);
        bannerNameLabel.AddToClassList("banner-name");

        // Создаём кнопку выбора отдельно (она больше не добавляется в баннер)
        Button selectButton = new Button(() => {
            Debug.Log($"Banner with {(isLayout ? "layout" : "style")} {bannerName} selected.");
            if (isLayout)
                container.ResolveNamed<LayoutService>(ServiceNames.LayoutSetting).SetCurrentItem(index);
            else
                container.ResolveNamed<StyleService>(ServiceNames.StyleSetting).SetCurrentItem(index);
            
            container.ResolveNamed<IRestartable>(ServiceNames.IRestartable_EdixorWindow).RestartWindow();
        }) { text = "Select" };
        selectButton.AddToClassList("select-button");

        // Добавляем элементы в контейнер в нужном порядке:
        // сверху имя баннера, затем сам баннер, и внизу кнопку выбора.
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
        VisualElement functionContainer = root.Q<VisualElement>("funcion-container"); 
        VisualElement functionSettingContainer = root.Q<VisualElement>("funcion-setting-container"); 
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
                    Debug.LogError("Ошибка: backgroundImage не является Texture2D");
                }

                function.AddToClassList("function");
                functionContainer.Add(function);
            }
        }
    }
}
