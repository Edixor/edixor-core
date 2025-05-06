// ExTools/DebugItem.cs
using System;

namespace ExTools
{
    public abstract class DebugItem 
    {
        public string Header { get; protected set; }
        public DateTime Timestamp { get; protected set; }
        public DebugSeverity Severity { get; protected set; }

        protected DebugItem(string header, DebugSeverity severity)
        {
            Header = header;
            Severity = severity;
            Timestamp = DateTime.Now;
        }
    }
}
