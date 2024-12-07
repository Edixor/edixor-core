using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BFPStyleModel))]
public class ExUIStyleEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        BFPStyleModel model = (BFPStyleModel)target;
        if (GUILayout.Button("Open in BFP Style Editor")) {
            ExUIStyleWindow window = EditorWindow.GetWindow<ExUIStyleWindow>();
            window.currentModel = model;
        }
    }
}