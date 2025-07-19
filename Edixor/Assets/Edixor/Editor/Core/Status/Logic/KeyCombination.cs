using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ExTools;

namespace ExTools
{
    public class KeyCombination : ExStatus
    {
        private readonly List<KeyCode> _keys = new List<KeyCode>();
        private Label _label;
        private readonly string _default = "Waiting for key presses…";
        private InputReader _input;
        private IHotKeyController _hotKeyController;
        private string _lastAcceptedName;

        public override VisualElement LoadUI()
        {
            if (item != null)
                return item;

            item = new VisualElement();
            item.AddToClassList("status-bar-item");
            _label = new Label(_default);
            item.Add(_label);
            return item;
        }

        private void Awake()
        {
            _keys.Clear();
            _lastAcceptedName = null;
            if (_label != null)
                _label.text = _default;
        }

        private void Start()
        {
            _label.text = _default;
            _input = container.ResolveNamed<InputReader>(container.Resolve<ServiceNameResolver>().Edixor_Input);
            _hotKeyController = container.ResolveNamed<IHotKeyController>(container.Resolve<ServiceNameResolver>().HotKeyController);
        }

        private void OnEnable()
        {
            _input.OnKeyDown += OnKeyDown;
            _input.OnKeyUp += OnKeyUp;
            if (_hotKeyController != null)
                _hotKeyController.OnHotKeyExecuted += OnHotKeyAccepted;
        }

        private void OnDisable()
        {
            _input.OnKeyDown -= OnKeyDown;
            _input.OnKeyUp -= OnKeyUp;
            if (_hotKeyController != null)
                _hotKeyController.OnHotKeyExecuted -= OnHotKeyAccepted;
        }

        private void OnKeyDown(KeyCode key)
        {
            if (key == KeyCode.None) return;

            _lastAcceptedName = null;

            if (!_keys.Contains(key))
            {
                _keys.Add(key);
                UpdateLabel();
            }
        }

        private void OnKeyUp(KeyCode key)
        {
            if (key == KeyCode.None) return;

            if (_keys.Remove(key))
            {
                if (_keys.Count == 0)
                {
                    _lastAcceptedName = null;
                    _label.text = _default;
                }
                else
                {
                    UpdateLabel();
                }
            }
        }

        private void OnHotKeyAccepted(string name)
        {
            _lastAcceptedName = name;
            _label.text = $"Activated: {name}";
        }

        private void UpdateLabel()
        {
            if (!string.IsNullOrEmpty(_lastAcceptedName))
            {
                _label.text = $"Activated: {_lastAcceptedName}";
            }
            else
            {
                _label.text = _keys.Count > 0 ? string.Join(" + ", _keys) : _default;
            }
        }
    }
}
