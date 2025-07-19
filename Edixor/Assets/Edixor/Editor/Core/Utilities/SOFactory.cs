using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ExTools
{
    public static class SOFactory
    {
        public static T CreateSO<T>(string assetPath) where T : ScriptableObject
        {
            var folder = Path.GetDirectoryName(assetPath);
            if (!string.IsNullOrEmpty(folder))
                EnsureFolderExists(folder);

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private static void EnsureFolderExists(string folderPath)
        {
            var parts = folderPath.Split('/');
            string acc = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                var next = $"{acc}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(acc, parts[i]);
                acc = next;
            }
        }
    }
}
