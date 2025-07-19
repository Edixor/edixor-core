using System.Collections.Generic;
using System.IO;
using ExTools;
using UnityEditor;
using UnityEngine;
using UObject = UnityEngine.Object;
using System;

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
            { "Status", "Editor/Core/Status/Data" },
            { "ContainerTabs", "Editor/UI/Tabs/Data/Basic" },
            { "Tabs", "Editor/UI/Tabs/Data/Basic" },
            { "Tabs Advanced", "Editor/UI/Tabs/Data/Advanced" },
            { "Tabs Other", "Editor/UI/Tabs/Data/Other" },
            { "Builder", "Editor/UI/Builder" },
            { "Resources", "Editor/Resources" },
            { "WindowStates", "Editor/UI/UIEdixor/Windows/States"},
            { "InspectionStates", "Editor/UI/UIEdixor/Inspector/States"}
        };

        private static string _cachedEdixorRoot;

        public static T LoadObject<T>(string virtualPath) where T : UObject
        {
            string path = Resolve(virtualPath);
            if (string.IsNullOrEmpty(PathResolver.ResolvePath(path)))
            {
                ExDebug.LogError($"Failed to load asset: path not found for '{virtualPath}'.");
                return null;
            }

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                ExDebug.LogWarning($"{typeof(T).Name} not found at path: {path}");
            }

            return asset;
        }

        public static List<T> FindAssetsInFolder<T>(string virtualPath, Func<T, bool> filter = null) where T : UObject
        {
            List<T> results = new();

            string folderPath = Resolve(virtualPath);
            if (string.IsNullOrEmpty(folderPath))
                return results;

            string typeFilter = $"t:{typeof(T).Name}";
            string[] guids = AssetDatabase.FindAssets(typeFilter, new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);

                if (asset == null)
                    continue;

                if (filter == null || filter(asset))
                    results.Add(asset);
            }

            return results;
        }

        public static List<TCreate> FindAndCreateInstances<TFind, TCreate>(string virtualPath)
            where TFind : UObject
            where TCreate : class
        {
            List<TCreate> instances = new();

            string folderPath = Resolve(virtualPath);
            if (string.IsNullOrEmpty(folderPath))
                return instances;

            string typeFilter = $"t:{typeof(TFind).Name}";
            string[] guids = AssetDatabase.FindAssets(typeFilter, new[] { folderPath });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                TFind asset = AssetDatabase.LoadAssetAtPath<TFind>(assetPath);
                if (asset == null)
                    continue;

                Type assetType = asset is MonoScript monoScript
                    ? monoScript.GetClass()
                    : asset.GetType();

                if (assetType == null || !typeof(TCreate).IsAssignableFrom(assetType) || assetType.IsAbstract)
                    continue;

                object instance = typeof(ScriptableObject).IsAssignableFrom(assetType)
                    ? ScriptableObject.CreateInstance(assetType)
                    : Activator.CreateInstance(assetType);

                if (instance is TCreate tCreateInstance)
                    instances.Add(tCreateInstance);
            }

            return instances;
        }

        public static string Resolve(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                ExDebug.LogError("Path cannot be empty.");
                return null;
            }

            string relativePath = ExpandShortcuts(virtualPath);
            string edixorFolder = GetOrFindEdixorRoot();

            if (string.IsNullOrEmpty(edixorFolder))
            {
                ExDebug.LogError("Edixor folder not found in project.");
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
            string relativePath = Resolve(virtualTargetPath);
            if (string.IsNullOrEmpty(relativePath))
            {
                Debug.LogError($"ImportDirectory: failed to resolve path '{virtualTargetPath}'");
                return;
            }

            string absoluteTarget = Path.Combine(Application.dataPath, relativePath.Substring("Assets/".Length));

            Directory.CreateDirectory(absoluteTarget);

            foreach (var dir in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                var targetDir = dir.Replace(sourcePath, absoluteTarget);
                if (!Directory.Exists(targetDir))
                    Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                var dest = file.Replace(sourcePath, absoluteTarget);
                File.Copy(file, dest, true);
            }

            AssetDatabase.Refresh();
        }

        private static string ExpandShortcuts(string path)
        {
            foreach (var kvp in Shortcuts)
            {
                if (path == kvp.Key)
                    return kvp.Value;

                if (path.StartsWith(kvp.Key + "/"))
                    return path.Replace(kvp.Key, kvp.Value);
            }
            return path;
        }

        public static string GetEdixorRootFolder()
        {
            return GetOrFindEdixorRoot();
        }
    }
}
