using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[Serializable]
public class RestartFunction : EdixorFunction, IFunctionSetting
{
    // Отображаем имя функции в инспекторе.
    // Можно, если нужно, сделать его редактируемым или только для чтения.
    [Header("Basic information")]
    [SerializeField]
    private string _functionName = "Restart";
    public string FunctionName
    {
        get { return _functionName; }
        private set { _functionName = value; }
    }

    // Поле для настройки "Tab cleaning" с заголовком "Настройки"
    [Header("Settings options")]
    [SerializeField]
    private bool _tabCleaning;
    public bool TabCleaning
    {
        get { return _tabCleaning; }
        set { _tabCleaning = value; }
    }

    [SerializeField]
    private bool _clearCache;
    public bool ClearCache
    {
        get { return _clearCache; }
        set { _clearCache = value; }
    }

    public RestartFunction(EdixorWindow window) : base(window)
    {
    }

    public override Texture2D Icon =>
        AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Edixor/Texture/EdixorWindow/Functions/photo_2_2024-12-24_18-59-17.jpg");

    // Переопределённое свойство Name возвращает значение нашего поля
    public override string Name => FunctionName;

    public override string Description => "Restarts the application or resets specific functionality.";

    public override void Activate()
    {
        if (Window != null)
        {
            Window.RestartWindow();
        }
    }

    // Метод для построения настроек через UIElements.
    // Здесь можно создать визуальное представление, используя заголовки и привязку к свойствам.
    public void Setting(VisualElement root)
    {
        Toggle tabCleaningToggle = new Toggle("Tab cleaning");

        tabCleaningToggle.RegisterValueChangedCallback(evt =>
        {
            TabCleaning = evt.newValue; 
        });

        Toggle clearCacheToggle = new Toggle("Clear cache");
        
        clearCacheToggle.RegisterValueChangedCallback(evt =>
        {
            ClearCache = evt.newValue;
        });

        root.Add(tabCleaningToggle);
        root.Add(clearCacheToggle);
    }

}

