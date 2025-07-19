public class OtherEdixorSetting : EdixorSettingData<OtherEdixorSettingAsset>
{
    public OtherEdixorSetting()
        : base("SettingAsset", "OtherEdixorSetting.asset")
    { }

    public string GetEdixorVersion() => Settings.EdixorVersion;
    public string GetLastChangelog() => Settings.LastChangelog;
    public string GetSupportedUnityVersions() => Settings.SupportedUnityVersions;

    public bool IsConsoleLoggingEnabled() => Settings.EnableConsoleLogging;
}
