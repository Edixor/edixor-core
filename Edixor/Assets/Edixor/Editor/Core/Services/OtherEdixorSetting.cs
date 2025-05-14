using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OtherEdixorSetting : EdixorSettingData<OtherEdixorSettingAsset>
{
    public OtherEdixorSetting() : base("SettingAsset/OtherEdixorSetting.asset") { }

    public bool IsConsoleLoggingEnabled() => Settings.EnableConsoleLogging;
}
