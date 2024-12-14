using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace Commands {
    public class CreateUSSFile : Command 
    {
        private string filePath;
        private string fileName;
        private string[] ussScriptContent;
        public CreateUSSFile(string filePath, string fileName, string[] ussScriptContent) {
            
            
            // если нету имени, выдается имя unnamed 
            if (fileName != null) {
                this.fileName = fileName;
            }
            else {
                this.fileName = "unnamed.uss";
            }
            
            // если чел не записал путь папке, файл сохраняется в Assets
            if (filePath != null) {
                this.filePath = filePath;
            }
            else {
                this.filePath = "Assets/" + fileName;
            }
            
            
            this.ussScriptContent = ussScriptContent;
        }


        public override void Tasks()
        {
            // сама задача
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string line in ussScriptContent)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public override void JobRollback() 
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    Debug.Log($"Script {fileName}.cs rollback successful. File deleted.");
                }
                else
                {
                    Debug.LogWarning($"Rollback failed. File {filePath} does not exist.");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to rollback script {fileName}.cs: {ex.Message}");
            }
        }

        public override void CallAction(Action action = null) {
        }

        public override void RollbackAction(Action action = null) {

        }

        // хз что здесь надо писать но думаю так сойдет
        public override T Get<T>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return $"{fileName}    ({DateTime.Now:HH:mm:ss})" as T;
            }
            else
            {
                return null;
            }
        }

    }
}