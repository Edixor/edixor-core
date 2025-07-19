using System.Collections.Generic;

namespace ExTools.Settings 
{
    public interface ISettingsFacade
    {
        HotKeySetting HotKeySetting { get; }
        FunctionSetting FunctionSetting { get; }
        StatusSetting StatusSetting { get; }
        TabSetting TabSetting { get; }
        void ResetConfiguration();
        string EdixorId { get; }
    }
}
