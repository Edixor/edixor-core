using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace ExTools
{
    public class ExStatus
    {
        public MonoScript ScriptLogic { get; private set; }
        protected DIContainer container;
        private VisualElement _bar;
        protected VisualElement item;

        private Action _awake;
        private Action _start;
        private Action _onEnable;
        private Action _onDisable;
        private Action _onDestroy;
        private Action _update;
        private bool _lifecycleDelegatesSet = false;

        public ExStatus(MonoScript script = null, DIContainer container = null)
        {
            this.ScriptLogic = script;
            this.container = container;
        }

        public void Init(VisualElement bar)
        {
            if (bar == null)
                ExDebug.LogError("bar is null. Cannot initialize ExStatus.");
            
            _bar = bar;
            SetupLifecycleDelegates();
        }

        public virtual VisualElement LoadUI()
        {
            if (_bar == null)
                ExDebug.LogError("bar is null. Cannot load UI in ExStatus.");

            item = new VisualElement();
            item.AddToClassList("status-bar-item");
            item.style.flexDirection = FlexDirection.Row;
            item.style.alignItems = Align.Center;

            Label label = new Label("Status");
            label.style.marginRight = 5;
            item.Add(label);

            return item;
        }

        public void Hide()
        {
            if (_bar == null)
                ExDebug.LogError("bar is null. Cannot hide ExStatus.");

            if (item != null && _bar.Contains(item))
            {
                _bar.Remove(item);
                item = null;
            }
        }

        public void Show()
        {
            if (_bar == null)
                ExDebug.LogError("bar is null. Cannot show ExStatus.");

            if (item == null)
            {
                item = LoadUI();
            }

            if (!_bar.Contains(item))
            {
                _bar.Add(item);
            }
        }

        public ExStatus GetLogic()
        {
            if (ScriptLogic == null)
            {
                ExDebug.LogWarning("ScriptLogic is null — unable to create logic.");
                return null;
            }

            Type type = ScriptLogic.GetClass();
            if (type == null || !typeof(ExStatus).IsAssignableFrom(type))
            {
                ExDebug.LogWarning($"Script {ScriptLogic.name} does not contain a compatible class.");
                return null;
            }

            try
            {
                ExStatus instance = (ExStatus)Activator.CreateInstance(type);
                instance.container = container;
                instance.ScriptLogic = ScriptLogic;
                return instance;
            }
            catch (Exception ex)
            {
                ExDebug.LogError($"Failed to create logic instance from {ScriptLogic.name}: {ex.Message}");
                return null;
            }
        }

        private void SetupLifecycleDelegates()
        {
            if (_lifecycleDelegatesSet) return;
            Type type = GetType();
            _awake     = CreateDelegateIfExists(type, "Awake");
            _start     = CreateDelegateIfExists(type, "Start");
            _onEnable  = CreateDelegateIfExists(type, "OnEnable");
            _onDisable = CreateDelegateIfExists(type, "OnDisable");
            _onDestroy = CreateDelegateIfExists(type, "OnDestroy");
            _update    = CreateDelegateIfExists(type, "Update");
            _lifecycleDelegatesSet = true;
        }

        private Action CreateDelegateIfExists(Type type, string methodName)
        {
            MethodInfo mi = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (mi != null && mi.GetParameters().Length == 0 && mi.ReturnType == typeof(void))
                return (Action)Delegate.CreateDelegate(typeof(Action), this, mi);
            return null;
        }

        public void InvokeAwake()     { _awake?.Invoke(); }
        public void InvokeStart()     { _start?.Invoke(); }
        public void InvokeOnEnable()  { _onEnable?.Invoke(); }
        public void InvokeOnDisable() { _onDisable?.Invoke(); }
        public void InvokeOnDestroy() { _onDestroy?.Invoke(); }
        public void InvokeUpdate()    { _update?.Invoke(); }
    }
}
