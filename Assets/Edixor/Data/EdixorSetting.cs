using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 
using System.IO;
using System;  

public static class EdixorSetting
{
    private static EdixorBaseData baseDataPlaginInstance;

    // Метод для проверки инициализации
    private static void CheckInitialization()
    {
        if (baseDataPlaginInstance == null)
        {
            Initialize();
        }
    }

    // Метод для инициализации экземпляра
    public static void Initialize()
    {
        string folderName = "Edixor";
        string rootPath = Directory.GetDirectoryRoot(Application.dataPath);
        string mainPath = FindFolder(rootPath, folderName);

        if (string.IsNullOrEmpty(mainPath))
        {
            Debug.LogError($"Folder '{folderName}' not found.");
        }
        else
        {
            Debug.Log($"Folder '{folderName}' found at: {mainPath}");
            // Добавляем путь к файлу
            string assetPath = Path.Combine(mainPath, "Data/BFPSetting.asset");

            Debug.Log(assetPath);

            string relativeAssetPath = "Assets/Edixor/Data/BFPSetting.asset";
            baseDataPlaginInstance = AssetDatabase.LoadAssetAtPath<EdixorBaseData>(relativeAssetPath);

            if (baseDataPlaginInstance == null)
            {
                Debug.LogError($"BaseDataPlagin not found at path: {relativeAssetPath}");
            }
            else
            {
                Debug.Log($"BaseDataPlagin successfully loaded from: {relativeAssetPath}");
            }
        }
    }

    private static string FindFolder(string root, string folderName)
    {
        try
        {
            foreach (string dir in Directory.GetDirectories(root))
            {
                if (dir.EndsWith(folderName))
                {
                    return dir;
                }

                string found = FindFolder(dir, folderName);
                if (!string.IsNullOrEmpty(found))
                {
                    return found;
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Обработка исключений
        }
        catch (Exception)
        {
            // Общая обработка исключений
        }
        return null;
    }

    // Пример метода для получения версии
    public static string GetVersion()
    {
        CheckInitialization(); // Проверка инициализации
        return baseDataPlaginInstance?.Version ?? "Instance not initialized";
    }

    // Пример метода для получения основного пути
    public static string GetMainPath()
    {
        CheckInitialization(); // Проверка инициализации
        return baseDataPlaginInstance?.MainPath ?? "Instance not initialized";
    }

    // Пример метода для установки версии
    public static void SetVersion(string version)
    {
        CheckInitialization(); // Проверка инициализации
        baseDataPlaginInstance?.SetVersion(version);
    }

    // Пример метода для установки основного пути
    public static void SetMainPath(string path)
    {
        CheckInitialization(); // Проверка инициализации
        baseDataPlaginInstance?.SetMainPath(path);
    }

    // Добавьте другие методы для работы с публичными переменными и методами
    public static List<DataPlagin> GetPlagins()
    {
        CheckInitialization(); // Проверка инициализации
        return baseDataPlaginInstance?.Plagins;
    }

    public static DataTranslation GetLanguages()
    {
        CheckInitialization(); // Проверка инициализации
        return baseDataPlaginInstance?.Languages;
    }

    public static void InitializeMainPath()
    {
        CheckInitialization(); // Проверка инициализации
        baseDataPlaginInstance?.InitializeMainPath();
    }

    public static void AddBFPStyle(BFPStyleModel model) {
        Debug.Log("2");
        CheckInitialization();
        baseDataPlaginInstance?.dataStyle.NewStyle(new BFPStyle(model));
    }
}
