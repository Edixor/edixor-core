 
using System;

namespace ExTools
{
    public class DebugEntry : DebugItem
    {
        public string StackTrace { get; }
        public string Caller { get; }
        public int LineNumber { get; }

        public DebugEntry(string message, DebugSeverity severity, string stackTrace, string caller, int lineNumber)
            : base(message, severity)
        {
            StackTrace = stackTrace;
            Caller = caller;
            LineNumber = lineNumber;
        }
    }
}
