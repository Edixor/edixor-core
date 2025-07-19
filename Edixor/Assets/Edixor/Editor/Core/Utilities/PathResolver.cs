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
            ExDebug.LogError($"[PathResolver] File '{fileName}' not found in project.");
            return null;
        }
        else if (allFiles.Length == 1)
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            ExDebug.LogWarning($"[PathResolver] Incorrect path. Use: {correctPath} instead of {path}");
            return correctPath;
        }
        else
        {
            string correctPath = allFiles[0].Replace("\\", "/");
            ExDebug.LogWarning($"[PathResolver] Multiple files found with name '{fileName}', first found path: {correctPath}");
            return correctPath;
        }
    }
}
