using UnityEngine;
using UnityEditor;
using System.IO;
using ExTools;

public static class PathResolver
{
    public static string ResolvePath(string path)
    {
        if (File.Exists(path))
        {
            return path;
        }

        string fileName = Path.GetFileName(path);
        string[] allFiles = Directory.GetFiles("Assets", fileName, SearchOption.AllDirectories);

        if (allFiles.Length == 0)
        {
            ExDebug.LogError($"[PathResolver] Файл '{fileName}' не найден в проекте.");
            return null;
        }
        else if (allFiles.Length == 1)
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            ExDebug.LogWarning($"[PathResolver] Путь некорректен. Используйте: {correctPath}, заместь {path}");
            return correctPath;
        }
        else
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            ExDebug.LogWarning($"[PathResolver] Найдено несколько файлов с именем '{fileName}', первый найденный путь: {correctPath}");
            return correctPath;
        }
    }
} 
