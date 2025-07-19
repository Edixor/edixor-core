 
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace ExTools
{
    public class InputReader
    {
        public event Action<KeyCode> OnKeyDown;
        public event Action<KeyCode> OnKeyUp;
        private readonly HashSet<KeyCode> _pressed = new HashSet<KeyCode>();

        public InputReader(VisualElement root)
        {
            root.focusable = true;
            root.pickingMode = PickingMode.Position;
            root.RegisterCallback<KeyDownEvent>(e =>
            {
                var k = e.keyCode;
                if (_pressed.Add(k)) OnKeyDown?.Invoke(k);
            });
            root.RegisterCallback<KeyUpEvent>(e =>
            {
                var k = e.keyCode;
                if (_pressed.Remove(k)) OnKeyUp?.Invoke(k);
            });
            root.Focus();
        }

        public IReadOnlyCollection<KeyCode> PressedKeys => _pressed;
    }
}
