using UnityEngine;
using UnityEditor;
using System.IO;

public static class PathResolver
{
    public static string ResolvePath(string path)
    {
        if (File.Exists(path))
        {
            return path; // Путь уже корректен
        }

        string fileName = Path.GetFileName(path);
        string[] allFiles = Directory.GetFiles("Assets", fileName, SearchOption.AllDirectories);

        if (allFiles.Length == 0)
        {
            Debug.LogError($"[PathResolver] Файл '{fileName}' не найден в проекте.");
            return null;
        }
        else if (allFiles.Length == 1)
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            Debug.LogWarning($"[PathResolver] Путь некорректен. Используйте: {correctPath}");
            return correctPath;
        }
        else
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            Debug.LogWarning($"[PathResolver] Найдено несколько файлов с именем '{fileName}', первый найденный путь: {correctPath}");
            return correctPath;
        }
    }
} 
