using UnityEngine;
using UnityEditor;
using ExTools;
using System;

namespace Commands {
    public class EditScriptObject<T> : Command where T : ScriptableObject
    {
        private string name;
        private DateTime currentTime;

        private Action createAction;
        private Action deleteAction;

        private object[] data;
        private object[] previousData;

        private T scriptableObject;

        private int index;

        public EditScriptObject(object[] data, T scriptableObject, int index, string name = null, Action createAction = null, Action deleteAction = null)
        {
            this.createAction = createAction;
            this.deleteAction = deleteAction;

            this.scriptableObject = scriptableObject;
            this.index = index;
            this.data = data;

            this.name = !string.IsNullOrEmpty(name)
                ? name
                : "Edit scriptable object (" + scriptableObject.GetType().ToString() + ")";
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now;
            var fields = scriptableObject.GetType().GetFields();

            if (fields.Length != data.Length)
            {
                ExDebug.LogError("The number of fields in the ScriptableObject does not match the number of elements in the array.");
                return;
            }

            previousData = data;

            for (int i = 0; i < fields.Length; i++)
            {
                if (data[i] == null)
                {
                    ExDebug.LogWarning($"Element at index {i} is null.");
                    continue;
                }

                if (fields[i].FieldType == data[i].GetType())
                {
                    fields[i].SetValue(scriptableObject, data[i]);
                }
                else
                {
                    ExDebug.LogWarning($"Field type mismatch: {fields[i].Name} ({fields[i].FieldType}) vs data type ({data[i].GetType()}).");
                }
            }
        }

        public override void JobRollback()
        {
            var fields = scriptableObject.GetType().GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == previousData[i].GetType())
                {
                    fields[i].SetValue(scriptableObject, previousData[i]);
                }
                else
                {
                    ExDebug.LogWarning($"Field type mismatch: {fields[i].Name} ({fields[i].FieldType}) vs previous data type ({previousData[i].GetType()}).");
                }
            }
        }

        public override void CallAction(Action action = null) { }

        public override void RollbackAction(Action action = null) { }

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

            return null;
        }
    }
}
