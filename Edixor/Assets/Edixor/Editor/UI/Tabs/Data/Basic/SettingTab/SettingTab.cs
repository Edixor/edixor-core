using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

using ExTools.Edixor.Interface;
using ExTools.Settings;
using ExTools.UI;
using ExTools;

namespace ExTools.Tabs.Basic
{
    public class SettingTab : EdixorTab
    {
        private FunctionSetting _functionSave;
        private StyleSetting _styleSetting;

        [MenuItem("Edixor/Tabs/Setting")]
        public static void ShowTab() => ShowTab<SettingTab>();

        private void Awake()
        {
            tabName = "Setting";
            LoadUxml("auto");
            LoadUss("auto");
            LoadIcon("Resources/Images/Icons/sett.png");
        }

        
        private void OnEnable()
        {
            if (root == null) return;          
            if (container == null) return;        

            _functionSave = container.ResolveNamed<ISettingsFacade>(
                                container.Resolve<ServiceNameResolver>().EdixorSettings
                        ).FunctionSetting;
            _styleSetting = container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting);

            var layoutContainer = root.Q<VisualElement>("layout-container");
            if (layoutContainer != null)
                RebuildLayouts();

            var styleContainer = root.Q<VisualElement>("style-container");
            if (styleContainer != null)
                RebuildStyles();
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

            var layouts = container.ResolveNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting).GetAllItem();
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
            var currentLayout = container.ResolveNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting).GetCorrectItem();
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

        private VisualElement CreateBanner(object dataObj, object otherObj, int index, bool isLayout)
        {
            var bannerContainer = new VisualElement();
            bannerContainer.AddToClassList("banner-container");

            var banner = new VisualElement();
            banner.AddToClassList("banner");
            banner.AddToClassList("middle-section");

            var styleData  = isLayout ? (StyleData)otherObj  : (StyleData)dataObj;
            var layoutData = isLayout ? (LayoutData)dataObj : (LayoutData)otherObj;

            var edixorDesign = new EdixorDesign(styleData, layoutData, banner, container);
            edixorDesign.LoadUI(true);

            var edixParams = styleData.GetEdixorParameters();

            var bannerStyle = edixParams.GetStyleByName(edixParams.Layout, "banner-box");
            bannerStyle?.Style.ApplyWithStates(banner);

            string name = isLayout
                ? ((LayoutData)dataObj).Name
                : ((StyleData)dataObj).Name;

            var nameLabel = new Label(name);
            nameLabel.AddToClassList("banner-name");
            var labelStyle = edixParams.GetStyleByName(edixParams.ContentLabels, "E-label");
            labelStyle?.Style.ApplyWithStates(nameLabel);

            var selectBtn = new Button(() =>
            {
                if (isLayout)
                    container.ResolveNamed<LayoutSetting>(ServiceNameKeys.LayoutSetting).UpdateIndex(index);
                else
                    container.ResolveNamed<StyleSetting>(ServiceNameKeys.StyleSetting).UpdateIndex(index);

                container.ResolveNamed<IRestartable>(
                    container.Resolve<ServiceNameResolver>().Edixor_Restart
                    ).RestartWindow();
            })
            {
                text = "Select"
            };
            selectBtn.AddToClassList("select-button");
            selectBtn.AddToClassList("E-button");
            var buttonStyle = edixParams.GetStyleByName(edixParams.ContentButtons, "E-button");
            buttonStyle?.Style.ApplyWithStates(selectBtn);

            bannerContainer.Add(nameLabel);
            bannerContainer.Add(banner);
            bannerContainer.Add(selectBtn);

            return bannerContainer;
        }
    }
}
