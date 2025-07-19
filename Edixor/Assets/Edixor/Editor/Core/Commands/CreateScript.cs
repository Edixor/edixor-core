using System;
using System.IO;
using UnityEngine;
using ExTools;


namespace Commands
{
    public class CreateScript : Command
    {
        private string name;
        private string nameScript;
        private string[] content;
        private string filePath;

        public CreateScript(string[] content, string filePath, string nameScript, string name = null)
        {
            this.content = content;
            this.name = string.IsNullOrEmpty(name) ? "Create script(" + nameScript + ")" : name;
            this.nameScript = nameScript;

            this.filePath = Path.Combine(Application.dataPath, $"{this.nameScript}.cs");
        }


        public override void Tasks()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    ExDebug.LogWarning($"File {filePath} already exists. Skipping creation.");
                    return;
                }

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (string line in content)
                    {
                        writer.WriteLine(line);
                    }
                }

                ExDebug.Log($"Script {nameScript}.cs created successfully at {filePath}");
            }
            catch (Exception ex)
            {
                ExDebug.LogError($"Failed to create script {nameScript}.cs: {ex.Message}");
            }
        }


        public override void JobRollback()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    ExDebug.Log($"Script {nameScript}.cs rollback successful. File deleted.");
                }
                else
                {
                    ExDebug.LogWarning($"Rollback failed. File {filePath} does not exist.");
                }
            }
            catch (Exception ex)
            {
                ExDebug.LogError($"Failed to rollback script {nameScript}.cs: {ex.Message}");
            }
        }


        public override void CallAction(Action action = null)
        {
            action?.Invoke();
        }


        public override void RollbackAction(Action action = null)
        {
            action?.Invoke();
        }


        public override T Get<T>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return $"{name}    ({DateTime.Now:HH:mm:ss})" as T;
            }
            else
            {
                return null;
            }
        }
    }
}
