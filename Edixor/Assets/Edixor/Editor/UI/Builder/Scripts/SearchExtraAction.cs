using System;

namespace ExTools
{    
    public class SearchExtraAction
    {
        public string IconPath { get; }
        public Func<string, bool> Action { get; }
        public string Tooltip { get; }

        public SearchExtraAction(string iconPath, Func<string, bool> action, string tooltip = null)
        {
            IconPath = iconPath;
            Action = action;
            Tooltip = tooltip;
        }
    }
}
