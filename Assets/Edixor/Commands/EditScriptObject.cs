using UnityEngine;
using UnityEditor;
using System;

namespace Commands {
    public class EditScriptObject<T> : Command where T : ScriptableObject
    {
        private string name;
        private DateTime currentTime;

        private Action TcrAction;
        private Action TdeAction;

        private object[] data;
        private object[] dataOld;

        private T scriptableObject;

        private int index;

        public EditScriptObject(object[] data, T scriptableObject, int index, string name = null, Action TcrAction = null, Action TdeAction = null)
        {
            this.TcrAction = TcrAction;
            this.TdeAction = TdeAction;

            this.scriptableObject = scriptableObject;

            this.index = index;
            this.data = data;

            if (!string.IsNullOrEmpty(name))
            {
                this.name = name;
            }
            else
            {
                this.name = "Edit script-object(" + scriptableObject.GetType().ToString() + ")";
            }
        }

        public override void Tasks()
        {
            currentTime = DateTime.Now; 
            var fields = scriptableObject.GetType().GetFields();

            if (fields.Length != data.Length)
            {
                Debug.LogError("Количество переменных в ScriptableObject и количество элементов в массиве не совпадает.");
                return;
            }

            dataOld = data;

            for (int i = 0; i < fields.Length; i++)
            {
                Debug.Log(fields[i].FieldType);

                if (data[i] != null)
                {
                    Debug.Log(data[i].GetType());
                }
                else
                {
                    Debug.LogWarning($"Элемент на индексе {i} равен null.");
                    continue; // Переход к следующей итерации, если элемент равен null
                }

                if (fields[i].FieldType == data[i].GetType())
                {
                    fields[i].SetValue(scriptableObject, data[i]);
                }
                else
                {
                    Debug.LogWarning($"Тип переменной {fields[i].Name} ({fields[i].FieldType}) не совпадает с типом элемента массива ({data[i].GetType()}).");
                }
            }
        }

        public override void JobRollback() 
        {
            var fields = scriptableObject.GetType().GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                if (fields[i].FieldType == dataOld[i].GetType())
                {
                    fields[i].SetValue(scriptableObject, dataOld[i]);
                }
                else
                {
                    Debug.LogWarning($"Тип переменной {fields[i].Name} ({fields[i].FieldType}) не совпадает с типом элемента массива ({dataOld[i].GetType()}).");
                }
            }
        }

        public override void CallAction(Action action = null) {

        }
        
        public override void RollbackAction(Action action = null) {

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
            else
            {
                return null; 
            }
        }
    }
}