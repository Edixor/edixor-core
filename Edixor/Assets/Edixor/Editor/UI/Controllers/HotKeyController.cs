using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

using ExTools.Settings;
using ExTools;

namespace ExTools.Controllers
{
    public class HotKeyController : ListControllerBase<KeyAction>, IHotKeyController
    {
        public event Action<string> OnHotKeyExecuted;

        private readonly DIContainer _container;
        private readonly InputReader _inputReader;
        private HashSet<KeyCode> _currentlyPressedKeys = new HashSet<KeyCode>();
        private HashSet<List<KeyCode>> _activatedCombinations = new HashSet<List<KeyCode>>(new KeyCombinationComparer());

        public HotKeyController(DIContainer container, ISettingsFacade settings)
        {
            _container = container;
            string id = settings.EdixorId;
            _inputReader = container.ResolveNamed<InputReader>(container.Resolve<ServiceNameResolver>().Edixor_Input);
            _inputReader.OnKeyDown += key => UpdatePressed(key, true);
            _inputReader.OnKeyUp += key => UpdatePressed(key, false);
        }

        public void InitHotKeys()
        {
            foreach (KeyAction action in items)
            {
                if (!action.Logic.Empty())
                {
                    action.Logic.SetContainer(_container);
                    action.Logic.SetScript(action.Data.ScriptLogic);
                }
            }
        }

        public bool IsHotKeyEnabled() => _currentlyPressedKeys != null;
        public override void Process() => OnKeys();

        public void OnKeys()
        {
            foreach (KeyAction action in items)
            {
                var combo = action.Data?.Combination;
                if (combo == null) continue;
                if (combo.All(_currentlyPressedKeys.Contains)
                    && !_activatedCombinations.Contains(combo)
                    && action.Data.enable)
                {
                    action.Activate();
                    _activatedCombinations.Add(combo);
                    OnHotKeyExecuted?.Invoke(action.Data.Name);
                }
            }
        }

        private void UpdatePressed(KeyCode key, bool down)
        {
            if (down) _currentlyPressedKeys.Add(key);
            else
            {
                _currentlyPressedKeys.Remove(key);
                _activatedCombinations.RemoveWhere(c => c.Contains(key));
            }
        }

        public void ResetConfiguration()
        {
            _currentlyPressedKeys.Clear();
            _activatedCombinations.Clear();
        }

        public override void AddItem(KeyAction action)
        {
            if (action == null || action.Data?.Combination == null) return;
            action.Logic.SetContainer(_container);
            base.AddItem(action);
        }

        private class KeyCombinationComparer : IEqualityComparer<List<KeyCode>>
        {
            public bool Equals(List<KeyCode> x, List<KeyCode> y)
            {
                if (x == null || y == null || x.Count != y.Count) return false;
                for (int i = 0; i < x.Count; i++)
                    if (x[i] != y[i]) return false;
                return true;
            }

            public int GetHashCode(List<KeyCode> obj)
            {
                unchecked
                {
                    int h = 17;
                    foreach (var k in obj) h = h * 31 + (int)k;
                    return h;
                }
            }
        }
    }
}