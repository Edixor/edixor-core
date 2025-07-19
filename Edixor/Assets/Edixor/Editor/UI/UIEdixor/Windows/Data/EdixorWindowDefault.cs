using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

using ExTools.Tabs.Basic;
using ExTools;

namespace ExTools.Windows
{
    public class EdixorWindowDefault : EdixorWindow
    {
        [MenuItem("Window/Edixor Window/EdixorDefault")]
        [MenuItem("Edixor/Window/EdixorDefault")]
        public static void ShowExample() => ShowWindow<EdixorWindowDefault>("EdixorDefault");
    }
    
}

