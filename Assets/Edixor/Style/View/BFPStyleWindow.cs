using UnityEditor;
using UnityEngine;

public class ExUIStyleWindow : EditorWindow {
    public BFPStyleModel currentModel;
    private BFPStyle style;

    [MenuItem("Window/BFP Style Editor")]
    public static void ShowWindow() {
        GetWindow<ExUIStyleWindow>("BFP Style Editor");
    }

    private void OnEnable() {
        if (currentModel != null) {
            style = new BFPStyle(currentModel);
        }
    }

    private void OnGUI() {
        if (currentModel == null) {
            EditorGUILayout.HelpBox("Select or create a BFP Style Model", MessageType.Info);
            if (GUILayout.Button("Create New Model")) {
                currentModel = CreateInstance<BFPStyleModel>();
                currentModel.styleName = "New Style";
                style = new BFPStyle(currentModel);
            }
        } else {
            currentModel.styleName = EditorGUILayout.TextField("Style Name", currentModel.styleName);
            
            // Поля для компонентов стиля
            if (currentModel.components != null) {
                for (int i = 0; i < currentModel.components.Length; i++) {
                    EditorGUILayout.LabelField($"Component {i + 1}: {currentModel.components[i].name}");
                }
            }

            if (GUILayout.Button("Save Model")) {
                // Логика сохранения модели
                AssetDatabase.CreateAsset(currentModel, $"Assets/BaseFacadeTPlagins/Style/StyleObject/{currentModel.styleName}.asset");
                AssetDatabase.SaveAssets();

                // Получаем ссылку на созданный ScriptableObject
                BFPStyleModel model = AssetDatabase.LoadAssetAtPath<BFPStyleModel>($"Assets/BaseFacadeTPlagins/Style/StyleObject/{currentModel.styleName}.asset");

                EdixorSetting.AddBFPStyle(model);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
