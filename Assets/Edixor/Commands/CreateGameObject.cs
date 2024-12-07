using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace Commands {
    public class CreateGameObject : Command
    {
        private string name;
        private string nameTasks;
        private DateTime currentTime;
        private GameObject gameObject;
        private GameObject prefab;
        private string namePrefab;
        private bool existenceParent;
        private Canvas canvas;
        private GameObject parentGameObject;

        public CreateGameObject(GameObject prefab, string name, Canvas canvas, string nameTasks = null)
        {
            this.canvas = canvas;
            this.prefab = prefab;
            this.namePrefab = name;

            if (!string.IsNullOrEmpty(nameTasks))
            {
                this.nameTasks = nameTasks;
            }
            else
            {
                this.nameTasks = "Create game-object(" + namePrefab + ")";
            }
        }

        public CreateGameObject(GameObject prefab, string name, GameObject parentGameObject, string nameTasks = null)
        {
            this.prefab = prefab;
            this.namePrefab = name;
            this.parentGameObject = parentGameObject;

            if (!string.IsNullOrEmpty(nameTasks))
            {
                this.nameTasks = nameTasks;
            }
            else
            {
                this.nameTasks = "Create game-object(" + namePrefab + ")";
            }
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now; 
            if (canvas != null)
            {
                gameObject = GameObject.Instantiate(prefab);

                if (namePrefab != null)
                {
                    gameObject.name = namePrefab;
                }

                gameObject.transform.SetParent(canvas.transform, false);
            }
            else if (parentGameObject != null || existenceParent)
            {
                gameObject = GameObject.Instantiate(prefab);
                
                if (namePrefab != null)
                {
                    gameObject.name = namePrefab;
                }

                gameObject.transform.SetParent(parentGameObject.transform, false);
                existenceParent = true;
            } 
            else
            {
                Debug.LogError("///");
            }
        }

        public override void JobRollback() {
    #if UNITY_EDITOR
            UnityEngine.Object.DestroyImmediate(gameObject);
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
                return nameTasks + "    " + "(" + currentTime.ToString("HH:mm:ss") + ")" as T;
            }
            if (typeof(T) == typeof(GameObject))
            {
                return gameObject as T;
            }
            else
            {
                return null; 
            }
        }
    }
}