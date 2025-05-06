using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

namespace ExTools
{
    public static class ExDebug
    {
        private static readonly List<DebugItem> _history = new();
        private static readonly Stack<DebugGroup> _stack = new();
        private static bool _console = false;

        public static void InitSetting(bool console) {
            _console = console;
        }

        public static void BeginGroup(string header)
        {
            var group = new DebugGroup(header);
            if (_stack.Count > 0)
                _stack.Peek().Children.Add(group);
            else
                _history.Add(group);

            _stack.Push(group);
        }

        // Закончить текущую группу
        public static void EndGroup()
        {
            if (_stack.Count > 0)
                _stack.Pop();
        }

        public static void Log(string message) => AddEntry(message, DebugSeverity.Info);
        public static void LogComment(string message) => AddEntry(message, DebugSeverity.Comment);
        public static void LogWarning(string message) => AddEntry(message, DebugSeverity.Warning);
        public static void LogError(string message) => AddEntry(message, DebugSeverity.Error);

        private static void AddEntry(string message, DebugSeverity severity)
        {
            var st = new StackTrace(true);
            var frame = st.FrameCount > 1 ? st.GetFrame(1) : st.GetFrame(0);
            string caller = frame.GetMethod()?.DeclaringType?.FullName + "." + frame.GetMethod()?.Name;
            int line = frame.GetFileLineNumber();
            string trace = st.ToString();

            var entry = new DebugEntry(message, severity, trace, caller, line);

            // Если сейчас внутри группы — кладём в её Children,
            // иначе — в корневой список истории
            if (_stack.Count > 0)
                _stack.Peek().Children.Add(entry);
            else
                _history.Add(entry);

            if(!_console) return;

            switch (severity)
            {
                case DebugSeverity.Info:    Debug.Log(message); break;
                case DebugSeverity.Comment: Debug.Log($"[Comment] {message}"); break;
                case DebugSeverity.Warning: Debug.LogWarning(message); break;
                case DebugSeverity.Error:   Debug.LogError(message); break;
            }
        }


        // Возвращает всю историю (группы и записи)
        public static IReadOnlyList<DebugItem> GetHistory() => _history.AsReadOnly();

        // Очистить историю
        public static void ClearHistory() => _history.Clear();
    }
}
