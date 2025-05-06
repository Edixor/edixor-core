using System.Collections.Generic;
using System.IO;
using ExTools;
using UnityEditor;
using UnityEngine;

namespace ExTools 
{
    public static class EdixorObjectLocator
    {
        private static readonly Dictionary<string, string> Shortcuts = new()
        {
            { "Servers", "Editor/Core/Services" },
            { "SettingAsset", "Editor/Settings/Assets" },
            { "Extensions", "Editor/Extensions" },
            { "HotKeys", "Editor/Core/HotKeys/Data" },
            { "Functions", "Editor/Core/Functions/Data" },
            { "Tabs", "Editor/UI/Tabs/Data" }
        };

        private static string _cachedEdixorRoot;

        public static T LoadObject<T>(string virtualPath) where T : Object
        {
            string path = Resolve(virtualPath);
            if (string.IsNullOrEmpty(PathResolver.ResolvePath(path)))
            {
                ExDebug.LogError($"Не удалось загрузить asset: путь не найден для '{virtualPath}'.");
                return null;
            }

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                ExDebug.LogWarning($"{typeof(T).Name} не найден по пути: {path}");
            }

            return asset;
        }

        /// <summary>
        /// Возвращает абсолютный путь к asset-файлу с учетом псевдонимов.
        /// </summary>
        public static string Resolve(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                ExDebug.LogError("Путь не может быть пустым.");
                return null;
            }

            string relativePath = ExpandShortcuts(virtualPath);
            string edixorFolder = GetOrFindEdixorRoot();

            if (string.IsNullOrEmpty(edixorFolder))
            {
                ExDebug.LogError("Папка Edixor не найдена в проекте.");
                return null;
            }

            return Path.Combine(edixorFolder, relativePath).Replace("\\", "/");
        }

        private static string GetOrFindEdixorRoot()
        {
            if (!string.IsNullOrEmpty(_cachedEdixorRoot))
                return _cachedEdixorRoot;

            string[] folders = Directory.GetDirectories(Application.dataPath, "Edixor", SearchOption.AllDirectories);
            if (folders.Length == 0)
                return null;

            string path = folders[0];
            _cachedEdixorRoot = "Assets" + path.Replace(Application.dataPath, "").Replace("\\", "/");
            return _cachedEdixorRoot;
        }

        public static void ImportDirectory(string sourcePath, string virtualTargetPath)
        {
            // Resolve виртуальный путь в относительный путь в Assets/, например "Assets/Edixor/Editor/Extensions/MyPlugin"
            string relativePath = Resolve(virtualTargetPath);
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.LogError($"ImportDirectory: не удалось разрешить путь «{virtualTargetPath}»");
                return;
            }

            // Абсолютный путь на диске
            string absoluteTarget = Path.Combine(Application.dataPath, relativePath.Substring("Assets/".Length));

            // Создаём директорию, если нужно
            Directory.CreateDirectory(absoluteTarget);

            // Копируем все папки
            foreach (var dir in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                var targetDir = dir.Replace(sourcePath, absoluteTarget);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
            }

            // Копируем все файлы
            foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var dest = file.Replace(sourcePath, absoluteTarget);
                File.Copy(file, dest, true);
            }

            // Обновляем AssetDatabase, чтобы Unity увидела новые файлы
            AssetDatabase.Refresh();
        }

        private static string ExpandShortcuts(string path)
        {
            foreach (var kvp in Shortcuts)
            {
                if (path.StartsWith(kvp.Key + "/"))
                    return path.Replace(kvp.Key, kvp.Value);
            }

            return path;
        }
    }
}
