using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

using ExTools.Settings;
using ExTools;

namespace ExTools.Functions
{
    public class HotKeys : ExFunction
    {
        private KeyActionData[] hotkeys;
        private ITabController tabController;

        public override void Activate()
        {
            tabController = container.ResolveNamed<ITabController>(
                container.Resolve<ServiceNameResolver>(
                ).TabController);

            if (tabController == null)
            {
                ExDebug.LogError("TabController is null.");
                return;
            }

            HotKeyTab hotKeysTab = new HotKeyTab();
            tabController.AddTab(hotKeysTab);
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