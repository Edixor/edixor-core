#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using UnityEngine;
using System;

namespace Commands
{
    public class CreateFolder : Command 
    {
        private string name;
        private DateTime currentTime;
        private string folderPath;
        private string nameNewFolder;
        private string createdFolderPath;

        public CreateFolder(string folderPath, string nameNewFolder, string name = null)
        {
            this.folderPath = folderPath;
            this.nameNewFolder = nameNewFolder;

            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "Create folder(" + nameNewFolder + ")";
            }
        }

        public override void Tasks()
        {
    #if UNITY_EDITOR
            currentTime = DateTime.Now; 
            createdFolderPath = folderPath + "/" + nameNewFolder;

            if (!AssetDatabase.IsValidFolder(createdFolderPath))
            {
                AssetDatabase.CreateFolder(folderPath, nameNewFolder);
                Debug.Log($"<b><color=cyan>MetaGame</color></b>: Folder created: {createdFolderPath}");
            }
            else
            {
                Debug.LogWarning($"<b><color=cyan>MetaGame</color></b>: Folder already exists: {createdFolderPath}");
            }
    #endif
        }

        public override void JobRollback() {
    #if UNITY_EDITOR
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
    #endif
        }

        public override void CallAction(Action action = null) {
            
        }

        public override void RollbackAction(Action action = null) {
            
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