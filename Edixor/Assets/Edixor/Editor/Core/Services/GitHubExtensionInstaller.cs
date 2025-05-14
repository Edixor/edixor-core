using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking; 
using System.IO.Compression;
using UnityEngine; 
using UnityEditor;
using System.IO;
using ExTools;
using System;
public class GitHubExtensionInstaller : IExtensionInstaller
{
    public async Task InstallAsync(IndexEntry ext)
    {
        string tempZip = Path.Combine(Application.temporaryCachePath, ext.name + ".zip");
        string extractDir = Path.Combine(Application.temporaryCachePath, ext.name);

        using var req = UnityWebRequest.Get(ext.zipUrl);
        var oper = req.SendWebRequest();
        while (!oper.isDone) await Task.Yield();
        if (req.result != UnityWebRequest.Result.Success)
            throw new Exception(req.error);

        File.WriteAllBytes(tempZip, req.downloadHandler.data);
        if (Directory.Exists(extractDir)) Directory.Delete(extractDir, true);
        ZipFile.ExtractToDirectory(tempZip, extractDir);

        string sourceDir = ExtensionUtils.FindRootDirectory(extractDir, ext.name);
        EdixorObjectLocator.ImportDirectory(sourceDir, $"Extensions/{ext.name}");
        File.Delete(tempZip);
        AssetDatabase.Refresh();
    }

    public async Task UninstallAsync(IndexEntry ext)
    {
        try
        {
            Debug.Log($"[Uninstall] start for '{ext.name}'");

            // 1) Разрешаем виртуальный путь через EdixorObjectLocator
            //    Это даст нам что-то вроде "Assets/Edixor/Editor/Extensions/<ext.name>"
            string assetPath = EdixorObjectLocator.Resolve($"Extensions/{ext.name}");
            Debug.Log($"[Uninstall] resolved assetPath = '{assetPath}'");

            // 2) Удаляем через AssetDatabase
            Debug.Log($"[Uninstall] AssetDatabase.DeleteAsset('{assetPath}')...");
            bool deleted = AssetDatabase.DeleteAsset(assetPath);
            Debug.Log(deleted
                ? "[Uninstall] AssetDatabase.DeleteAsset succeeded"
                : "[Uninstall] AssetDatabase.DeleteAsset returned false (asset may not exist)");

            // 3) На всякий случай подчистим папку на диске
            //    Получаем относительный путь без "Assets/"
            if (!string.IsNullOrEmpty(assetPath) && assetPath.StartsWith("Assets/"))
            {
                string relative = assetPath.Substring("Assets/".Length);
                string fullDir = Path.Combine(Application.dataPath, relative);
                Debug.Log($"[Uninstall] fullDir = '{fullDir}' exists? {Directory.Exists(fullDir)}");
                if (Directory.Exists(fullDir))
                {
                    Debug.Log($"[Uninstall] Directory.Delete('{fullDir}', true)...");
                    Directory.Delete(fullDir, true);
                    Debug.Log("[Uninstall] Directory.Delete done");
                }
                else
                {
                    Debug.Log("[Uninstall] skipping Directory.Delete, folder not found");
                }
            }

            // 4) Обновляем Unity
            Debug.Log("[Uninstall] AssetDatabase.Refresh()");
            AssetDatabase.Refresh();
            Debug.Log("[Uninstall] done");
        }
        catch (Exception e)
        {
            Debug.LogError($"[Uninstall] Exception: {e.GetType().Name}: {e.Message}");
        }

        await Task.CompletedTask;
    }

}