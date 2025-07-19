using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ExTools.Tabs.Basic 
{
    public class DemonstrationTab : EdixorTab
    {
        [MenuItem("Edixor/Tabs/Demonstration UIStyle")]
        public static void ShowTab() => ShowTab<DemonstrationTab>();

        private void Awake()
        {
            Option("Demonstration UIStyle", "auto", "auto", "Resources/Images/Icons/edix.png");
        }
    }
}