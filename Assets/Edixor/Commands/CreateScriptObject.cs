using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Commands {
    public class CreateScriptObject<T> : Command where T : ScriptableObject
    {
        private string name;
        private DateTime currentTime;
        private Action TcrAction;
        private Action TdeAction;
        private Func<int, CreateScriptObject<T>> func;
        private string folderPath;
        private string assetName;
        private T scriptObject;
        private string assetPath;
        private int index;
        private bool arrayTask;

        public CreateScriptObject(bool arrayTask, string folderPath, string assetName, int index = -1, Func<int, CreateScriptObject<T>> func = null, Action TcrAction = null, Action TdeAction = null, string name = null) {

            this.folderPath = folderPath;
            this.assetName = assetName;

            this.TcrAction = TcrAction;
            this.TdeAction = TdeAction;
            this.func = func;

            this.index = index;
            this.arrayTask = arrayTask;
            
            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "Create script-object(" + assetName + ")";
            }
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now; 
            T asset = ScriptableObject.CreateInstance<T>();

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("<b><color=cyan>MetaGame</color></b>: The specified path does not exist: " + folderPath);
                return;
            }

            assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{assetName}.asset");

            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            scriptObject = asset;
            scriptObject.name = assetName;

            Debug.Log($"<b><color=cyan>MetaGame</color></b>: ScriptableObject created: {assetPath}");
        }

        public override void JobRollback() 
        {
            Debug.Log("CreateSO func:" + (func != null));
            new DestroyScriptObject<T>(arrayTask, folderPath, assetName, scriptObject, index, func, TdeAction, TcrAction).Tasks();
        }

        public override void CallAction(Action action = null) {
            TcrAction.Invoke();
        }
        
        public override void RollbackAction(Action action = null) {
            TdeAction.Invoke();
        }

        public override U Get<U>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return name + "    " + "(" + currentTime.ToString("HH:mm:ss") + ")" as U;
            }
            if (typeof(U) == typeof(string))
            {
                return index.ToString() as U;
            }
            else if (typeof(U) == scriptObject.GetType())
            {
                Debug.Log(index + " LLLLLLLLLLLL");
                Debug.Log("AAAAAAAAAAAAASSSSSSSSDDDDDDDD");
                Debug.Log(scriptObject.GetType());
                Debug.Log(scriptObject != null);
                if (scriptObject == null)
                {
                    Tasks();
                }
                return scriptObject as U;
            }
            else
            {
                return null; 
            }
        }
    }
}