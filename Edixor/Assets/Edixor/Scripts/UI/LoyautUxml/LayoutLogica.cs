using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System;

public abstract class LayoutLogica
{
    protected DIContainer container;
    protected abstract Function[][] functions { get; }
    protected abstract Dictionary<string, Function[]> elements { get; }
    private IFactory factoryBuilder;
    private List<Button> buttonFunc = new List<Button>();

    public void Init(VisualElement root)
    {

        VisualElement rootElement = root;

        foreach (var element in elements)
        {
            VisualElement uiElement = rootElement.Q<VisualElement>(element.Key);
            if (uiElement == null)
            {
                UnityEngine.Debug.LogWarning($"UI элемент с ключом '{element.Key}' не найден.");
                continue;
            }

            Function[] functionsArray = element.Value;

            foreach (var function in functionsArray)
            {
                Button functionButton = new Button();

                Texture2D backgroundImage = function.Data.Icon as Texture2D; 
                if (backgroundImage != null)
                {
                    functionButton.style.backgroundImage = new StyleBackground(backgroundImage);
                }
                else
                {
                    Debug.LogError("Ошибка: backgroundImage не является Texture2D");
                }


                functionButton.AddToClassList("function");

                functionButton.clicked += () =>
                {
                    function.Logic.SetContainer(container);
                    function.Logic.Init();
                    function.Logic.Activate();
                };
                
                uiElement.Add(functionButton);

                buttonFunc.Add(functionButton);
            }
        }
    }
    
    public void DemoInit(VisualElement root, Color icon, Color background)
    {
        VisualElement rootElement = root;

        VisualElement tabContainer = rootElement.Q<VisualElement>("tab-section");
        if (tabContainer != null)
        {
            VisualElement newTab = CreateTabContainer();
            tabContainer.Add(newTab);
        } 
        else
        {
            Debug.LogWarning("Контейнер для вкладок не найден.");
        }

        VisualElement middleSection = rootElement.Q<VisualElement>("middle-section-content");
        if (middleSection != null)
        {
            Label textLabel = new Label("text-content");
            middleSection.Add(textLabel);
        }
        else
        {
            Debug.LogWarning("Контейнер для текста не найден.");
        }

        foreach (var element in elements)
        {
            VisualElement uiElement = rootElement.Q<VisualElement>(element.Key);
            if (uiElement == null)
            {
                UnityEngine.Debug.LogWarning($"UI элемент с ключом '{element.Key}' не найден");
                continue;
            }

            Function[] functionsArray = element.Value;

            foreach (var function in functionsArray)
            {
                Button functionButton = new Button() { text = "<b>F</b>" };
                functionButton.enableRichText = true;

                functionButton.AddToClassList("function");
                functionButton.style.backgroundColor = background;
                functionButton.style.color = icon;
                functionButton.pickingMode = PickingMode.Ignore;

                uiElement.Add(functionButton);
            }
        }
    }


    private VisualElement CreateTabContainer()
    {
        VisualElement containerElement = new VisualElement();
        containerElement.AddToClassList("tab-container");

        var tabButton = new Button()
        {
            text = "tab"
        };
        tabButton.pickingMode = PickingMode.Ignore;
        tabButton.AddToClassList("tab-button");
        containerElement.Add(tabButton);

        var closeButton = new Button()
        {
            text = "X"
        };
        closeButton.pickingMode = PickingMode.Ignore;
        closeButton.AddToClassList("tab-button-exit");
        containerElement.Add(closeButton);

        return containerElement;
    }


    protected Function LoadFucntion(string name) {
        FunctionData functionData = LoadFucntionData(name);

        if (functionData == null) {
            throw new ArgumentNullException(nameof(functionData), $"Функция с именем '{name}' не найдена.");
        }

        factoryBuilder = container.Resolve<IFactory>();
        factoryBuilder.Init<FunctionData, FunctionLogica, Function>(data => data.Logica);
        return (Function)factoryBuilder.Create(functionData);
    }
    protected FunctionData LoadFucntionData(string name) {
        return container.ResolveNamed<FunctionService>(ServiceNames.FunctionSetting).GetCorrectFunction(name);
    }

    public void SetContainer(DIContainer container) {
        this.container = container;
    }

    public List<Button> GetFunctions() {
        return buttonFunc;
    }
}
