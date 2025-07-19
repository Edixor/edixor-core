using UnityEngine;
using System;

[CreateAssetMenu(fileName = "OtherEdixorSetting", menuName = "Edixor/Settings/Other Edixor Setting")]
public class OtherEdixorSettingAsset : ScriptableObject
{
    [SerializeField]
    private bool enableConsoleLogging = true;

    [SerializeField]
    private string edixorVersion = "1.0.0";

    [SerializeField]
    private string lastChangelog = "Initial release.";

    [SerializeField]
    private string supportedUnityVersions = "2021.3+, 2022.3+, 2023.1+";

    public static Action UpdateSetting;

    private bool _lastEnableConsoleLogging;

    private void OnValidate()
    {
        if (_lastEnableConsoleLogging != enableConsoleLogging)
        {
            _lastEnableConsoleLogging = enableConsoleLogging;
            UpdateSetting?.Invoke();
        }
    }

    public bool EnableConsoleLogging
    {
        get => enableConsoleLogging;
        set
        {
            if (enableConsoleLogging != value)
            {
                enableConsoleLogging = value;
                UpdateSetting?.Invoke();
            }
        }
    }

    public string EdixorVersion
    {
        get => edixorVersion;
        set => edixorVersion = value;
    }

    public string LastChangelog
    {
        get => lastChangelog;
        set => lastChangelog = value;
    }

    public string SupportedUnityVersions
    {
        get => supportedUnityVersions;
        set => supportedUnityVersions = value;
    }
}
