using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

using ExTools.Tabs.Advanced;
using ExTools.Settings;
using ExTools;

namespace ExTools.Inspector
{
    [CustomEditor(typeof(DemonstrationEdixorInspectorDefault))]
    public class EdixorInspectorDefault : EdixorInspector
    {
        protected override void Configure(WindowConfigurationBuilder builder)
        {
            builder
                .HotKey("Exit")
                .HotKey("Restart")

                .Function("HotKey")
                .Function("Setting")
                .Function("RestartInspector")

                .Status("Version")
                .Status("Key Combination")

                .BasicTab("ExtensionInspectTab")

                .Layout("Standard")
                .Style("Unity");
        }

        protected override void OnMainTab(VisualElement mainTab)
        {
            mainTab.Add(new Label("This is the default inspector for Edixor."));
            mainTab.Add(new Button(() =>
            {
            }));
        }
    }
}
