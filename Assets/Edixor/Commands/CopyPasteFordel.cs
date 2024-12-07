using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Commands
{
    public class CopyPasteFordel : Command 
    {
        private string name;
        private DateTime currentTime;
        private string pathCopy;
        private string pathPaste;
        private string nameNewFolder;
        private string createdFolderPath;

        public CopyPasteFordel(string pathCopy, string pathPaste, string nameNewFolder, string name = null)
        {
            this.pathCopy = pathCopy;
            this.pathPaste = pathPaste;
            this.nameNewFolder = nameNewFolder;

            if (!string.IsNullOrEmpty(name))
            {
                this.name = "Create and Copy folder (" + nameNewFolder + ")";
            }
            else
            {
                this.name = name;
            }
        }

        public override void Tasks()
        {
            try
            {
                currentTime = DateTime.Now; 
                createdFolderPath = Path.Combine(pathPaste, nameNewFolder);

                // Проверка существования директории
                if (!AssetDatabase.IsValidFolder(createdFolderPath))
                {
                    AssetDatabase.CreateFolder(pathPaste, nameNewFolder);
                    AssetDatabase.Refresh();

                    // Копирование содержимого из шаблонной папки
                    FileUtil.CopyFileOrDirectory(pathCopy, createdFolderPath);
                    AssetDatabase.Refresh();

                    Debug.Log($"<b><color=cyan>MetaGame</color></b>: Folder '{nameNewFolder}' created and contents copied from '{pathCopy}' to '{createdFolderPath}'");
                }
                else
                {
                    Debug.LogWarning($"<b><color=cyan>MetaGame</color></b>: Folder already exists: {createdFolderPath}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"<b><color=cyan>MetaGame</color></b>: Error creating or copying folder: {ex.Message}");
            }
        }

        public override void JobRollback() 
        {
            try
            {
                if (Directory.Exists(createdFolderPath))
                {
                    Directory.Delete(createdFolderPath, true);
                    Debug.Log($"<b><color=cyan>MetaGame</color></b>: Folder deleted: {createdFolderPath}");

                    string metaFilePath = createdFolderPath + ".meta";
                    if (File.Exists(metaFilePath))
                    {
                        File.Delete(metaFilePath);
                        Debug.Log($"<b><color=cyan>MetaGame</color></b>: .meta file deleted: {metaFilePath}");
                    }
                    else
                    {
                        Debug.LogWarning($"<b><color=cyan>MetaGame</color></b>: .meta file not found for folder: {createdFolderPath}");
                    }
                }
                else
                {
                    Debug.LogWarning($"<b><color=cyan>MetaGame</color></b>: Folder does not exist: {createdFolderPath}");
                }

                AssetDatabase.Refresh();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"<b><color=cyan>MetaGame</color></b>: Error deleting folder {createdFolderPath}: {ex.Message}");
            }
        }

        public override void CallAction(Action action = null) 
        {
            // Оставляем пустым или добавляем логику, если нужно
        }

        public override void RollbackAction(Action action = null) 
        {
            // Оставляем пустым или добавляем логику, если нужно
        }

        public override T Get<T>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return name + "    " + "(" + currentTime.ToString("HH:mm:ss") + ")" as T;
            }
            if (typeof(T) == typeof(string))
            {
                return createdFolderPath as T;
            }
            else
            {
                return null; 
            }
        }
    }
}
