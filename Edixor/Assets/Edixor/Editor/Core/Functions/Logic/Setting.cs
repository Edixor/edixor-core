using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using System;

using ExTools.Tabs.Basic;
using ExTools.Settings;
using ExTools;

namespace ExTools.Functions
{
    public class Setting : ExFunction
    {

        private ITabController tabController;

        public override void Activate()
        {
            tabController = container.ResolveNamed<ITabController>(container.Resolve<ServiceNameResolver>().TabController);

            if (tabController == null)
            {
                ExDebug.LogError("TabController is null.");
                return;
            }

            SettingTab settingTab = new SettingTab();
            tabController.AddTab(settingTab);
        }

        public override void Init()
        {
            if (container == null)
            {
                ExDebug.LogError("DIContainer is null.");
                return;
            }
            if (container.ResolveNamed<EdixorRegistrySetting>(ServiceNameKeys.EdixorRegistrySetting).GetCorrectItem() == null)
            {
                ExDebug.LogError("CurrentWindow is null.");
                return;
            }
        }
    }
}