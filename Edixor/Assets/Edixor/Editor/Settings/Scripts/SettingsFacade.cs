using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ExTools.Settings
{
    public class SettingsFacade : ISettingsFacade
    {
        public HotKeySetting HotKeySetting { get; }
        public FunctionSetting FunctionSetting { get; }
        public StatusSetting StatusSetting { get; }
        public TabSetting TabSetting { get; }
        public string EdixorId { get; private set; }

        public SettingsFacade(string edixorName, string edixorId, DIContainer container)
        {
            HotKeySetting = new HotKeySetting(container, edixorName);
            FunctionSetting = new FunctionSetting(container, edixorName);
            StatusSetting = new StatusSetting(container, edixorName);
            TabSetting = new TabSetting(edixorName);

            this.EdixorId = edixorId;
        }

        public void ResetConfiguration()
        {
            HotKeySetting.ResetConfiguration();
            FunctionSetting.ResetConfiguration();
            StatusSetting.ResetConfiguration();
            TabSetting.ResetConfiguration();
        }
    }
}
