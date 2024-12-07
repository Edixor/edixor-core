using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Commands {
    public class DestroyScriptObject<T> : Command where T : ScriptableObject
    {
        private string name;
        private DateTime currentTime;
        private Action TdeAction;
        private Action TcrAction;
        private Func<int, CreateScriptObject<T>> func;
        private CreateScriptObject<T> createScriptObject;
        private string folderPath;
        private string assetName;
        private T scriptObject;
        private int index;
        private bool arrayTask;

        public DestroyScriptObject(bool arrayTask, string folderPath, string assetName, T scriptObject, int index, Func<int, CreateScriptObject<T>> func, Action TdeAction, Action TcrAction, string name = null) {

            this.folderPath = folderPath;
            this.assetName = assetName;

            this.func = func;
            this.TdeAction = TdeAction;
            this.TcrAction = TcrAction;

            this.scriptObject = scriptObject;

            if(index !> 0) this.index = index;
            else this.index = 0;
            
            this.arrayTask = arrayTask;

            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "Destroy script-object(" + assetName + ")";
            }
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now; 

            if (scriptObject == null || arrayTask)
            { 
                Debug.Log(index);
                createScriptObject = func.Invoke(index);
                scriptObject = createScriptObject.Get<T>();
            }

            if (scriptObject != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(scriptObject);
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        public override void JobRollback() 
        {
            createScriptObject.Tasks();
        }

        public override void CallAction(Action action = null) {
            TdeAction.Invoke();
        }

        public override void RollbackAction(Action action = null) {

            scriptObject = createScriptObject.Get<T>("CreateScriptObject`1");
            createScriptObject.CallAction(TcrAction);
            //TcrAction.Invoke();
        }

        public override U Get<U>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return name + "    " + "(" + currentTime.ToString("HH:mm:ss") + ")" as U;
            }
            if (typeof(U) == scriptObject.GetType())
            {
                return scriptObject as U;
            }
            else
            {
                return null; 
            }
        }
    }
}