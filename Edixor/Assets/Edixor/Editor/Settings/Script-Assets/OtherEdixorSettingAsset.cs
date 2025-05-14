using UnityEngine;
using System;

[CreateAssetMenu(fileName = "OtherEdixorSetting", menuName = "Edixor/Settings/Other Edixor Setting")]
public class OtherEdixorSettingAsset : ScriptableObject
{
    [SerializeField]
    private bool enableConsoleLogging = true;

    public static Action UpdateSetting;

    private bool _lastValue;

    private void OnValidate()
    {
        if (_lastValue != enableConsoleLogging)
        {
            _lastValue = enableConsoleLogging;
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
}