using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Commands {
    public class MoveScriptObject<T> : Command where T : ScriptableObject
    {
        private string name;
        private DateTime currentTime;
        private Action TmsAction;

        private List<T> arraySO;

        private int index;
        private int jndex;

        public MoveScriptObject(List<T> array, int index, int jndex, Action action, string name = null) {

            arraySO = array;

            this.index = index;
            this.jndex = jndex;

            TmsAction = action;

            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "Move script object " + index + " > " + jndex;
            }
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now; 
            T temp = arraySO[index];
            arraySO[index] = arraySO[jndex];
            arraySO[jndex] = temp;
        }

        public override void JobRollback() 
        {
            T temp = arraySO[jndex];
            arraySO[jndex] = arraySO[index];
            arraySO[index] = temp;
        }

        public override void CallAction(Action action = null) {
            TmsAction.Invoke();
        }

        public override void RollbackAction(Action action = null) {
            TmsAction.Invoke();
        }

        public override U Get<U>(string clarifications = null)
        {
            if (clarifications == "taskName")
            {
                return name + "    " + "(" + currentTime.ToString("HH:mm:ss") + ")" as U;
            }
            if (typeof(U) == typeof(string))
            {
                return index + "/" + jndex as U;
            }
            else
            {
                return null;
            }    
        }
    }
}