// ExTools/DebugGroup.cs
using System.Collections.Generic;

namespace ExTools
{
    public class DebugGroup : DebugItem
    {
        public List<DebugItem> Children { get; } = new();
        public bool IsExpanded { get; set; } = false;

        public DebugGroup(string header)
            : base(header, DebugSeverity.Group)
        {
        }
    }

}
