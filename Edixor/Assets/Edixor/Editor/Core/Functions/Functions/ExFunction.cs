using System;
using UnityEngine;
using UnityEditor;

namespace ExTools
{
    [Serializable]
    public class ExFunction
    {
        public MonoScript ScriptLogic { get; private set; }
        protected DIContainer container;

        public ExFunction(MonoScript script = null, DIContainer container = null)
        {
            this.ScriptLogic = script;
            this.container = container;
        }

        public bool IsEmpty() => container == null;

        public void SetContainer(DIContainer container) => this.container = container;

        public void SetScript(MonoScript script)
        {
            ScriptLogic = script;
            if (container != null)
                Init();
        }

        public ExFunction GetLogic()
        {
            if (ScriptLogic == null)
            {
                ExDebug.LogWarning("ScriptLogic is null — cannot create logic instance.");
                return null;
            }

            var type = ScriptLogic.GetClass();
            if (type == null || !typeof(ExFunction).IsAssignableFrom(type))
            {
                ExDebug.LogWarning($"Script {ScriptLogic.name} does not contain a compatible class.");
                return null;
            }

            try
            {
                var instance = Activator.CreateInstance(type) as ExFunction;
                if (instance != null)
                {
                    instance.SetContainer(container);
                    instance.SetScript(ScriptLogic);
                }
                return instance;
            }
            catch (Exception ex)
            {
                ExDebug.LogError($"Error creating logic instance from {ScriptLogic.name}: {ex.Message}");
                return null;
            }
        }

        public virtual void Init()
        {
            if (container == null)
                throw new InvalidOperationException("DIContainer is not set. Please set it before calling Init.");
        }

        public virtual void Activate()
        {
            if (container == null)
                throw new InvalidOperationException("DIContainer is not set. Please set it before calling Activate.");
        }

        public virtual void Execute()
        {
            if (container == null)
            {
                ExDebug.LogError("Attempted to call Execute() without a DIContainer set.");
                return;
            }

            ExDebug.Log($"Executing function logic: {ScriptLogic?.name ?? "No script"}");
        }
    }
}
