using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;
using ExTools;
using System;
public class AddFuncTab : EdixorTab
{
    [MenuItem("Edixor/Tabs/Tab")]
    public static void ShowTab()
    {
        ShowTab<AddFuncTab>();
    }

    public void Awake() {
        
    }
}
