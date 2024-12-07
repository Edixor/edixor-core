/*using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BaseDataPlagin))]
public class EdixorData : Edixor
{
    private BFPStyleComponent styleWindow;

    public override void OnInspectorGUI()
    {
        BaseDataPlagin baseDataPlagin = (BaseDataPlagin)target;

        if (styleWindow == null)
        {
            BFPStyleComponent component = baseDataPlagin.Styles[0].GetStyleComponent("window");

            if (component is BFPStyleComponent componentWindow)
            {
                styleWindow = componentWindow;
                //styleWindow.ComponentInitialization(true);
            }
        }

        BFPLayout.Background(new Rect(0, 0, 5000, 5000));

        //var labelStyle = styleWindow.GetElementGUI("label", "normal");
        //var textFieldStyle = styleWindow.GetElementGUI("label", "textField");

        //var buttonStyle = styleWindow.GetElementGUI("button", "normal");

        //var boxStyle = styleWindow.GetElementGUI("box", "normal");

        GUI.skin.label = styleWindow.GetElementGUI(TypeElements.label, "norm");

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Index Language", GUILayout.Width(111));
        GUILayout.Space(5);
        baseDataPlagin.indexLanguage = EditorGUILayout.IntField(baseDataPlagin.indexLanguage);
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        if (baseDataPlagin.Languages != null && baseDataPlagin.Languages.Count > 0)
        {
            foreach (var language in baseDataPlagin.Languages)
            {
                EditorGUILayout.BeginVertical(); // Начало бокса для языка
                EditorGUILayout.BeginHorizontal(GUILayout.Height(30));

                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(language, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                GUILayout.FlexibleSpace();

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    Debug.Log($"Apply for language: {language}");
                }
                if (GUILayout.Button("Settings", GUILayout.Height(20)))
                {
                    Debug.Log($"Settings for language: {language}");
                }
                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical(); // Конец бокса для языка
            }
        }
        else
        {
            GUILayout.Label("none");
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Style");

        if (baseDataPlagin.Styles != null && baseDataPlagin.Styles.Count > 0)
        {
            foreach (var style in baseDataPlagin.Styles)
            {
                EditorGUILayout.BeginVertical(); // Начало бокса для стиля
                EditorGUILayout.BeginHorizontal(GUILayout.Height(30));

                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(6);
                EditorGUILayout.LabelField(style.styleName, GUILayout.MinWidth(100), GUILayout.MaxWidth(200));
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();

                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(6);
                if (GUILayout.Button("Apply", GUILayout.Height(20)))
                {
                    BFPStyleComponent component = baseDataPlagin.Styles[0].GetStyleComponent("window");

                    if (component is BFPStyleComponent newStyleWindow)
                    {
                        //newStyleWindow.ComponentInitialization(true);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(6);
                if (GUILayout.Button("Settings", GUILayout.Height(20)))
                {
                    Debug.Log($"Settings for style: {style.styleName}");
                }
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();
                GUI.enabled = baseDataPlagin.developerMode;
                EditorGUILayout.BeginVertical();
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Path", GUILayout.Width(40));
                GUILayout.Space(5);

                // Устанавливаем фиксированную ширину текстового поля
                float textFieldWidth = EditorGUIUtility.currentViewWidth - 102; // 60 для учета ширины метки и отступов
                baseDataPlagin.SetMainPath(GUILayout.TextField(baseDataPlagin.MainPath ?? "Not initialized", GUILayout.Width(textFieldWidth)));

                GUILayout.EndHorizontal();

                GUI.enabled = true;

                EditorGUILayout.EndVertical(); 

                EditorGUILayout.EndVertical(); 
            }
        }
        else
        {
            GUILayout.Label("none");
        }

        // Раздел: Остальное
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Info");
        GUI.enabled = baseDataPlagin.developerMode;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Version", GUILayout.Width(80));
        GUILayout.Space(5);
        baseDataPlagin.SetVersion(GUILayout.TextField(baseDataPlagin.Version ?? "Not specified"));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Path", GUILayout.Width(80));
        GUILayout.Space(5);
        baseDataPlagin.SetMainPath(GUILayout.TextField(baseDataPlagin.MainPath ?? "Not initialized"));
        GUILayout.EndHorizontal();

        //EditorGUILayout.TextField("Version", baseDataPlagin.Version ?? "Not specified", textStyle);
        //EditorGUILayout.TextField("Path", baseDataPlagin.MainPath ?? "Not initialized", textStyle);
        GUI.enabled = true;

        EditorGUILayout.Space();
        if (GUILayout.Button("Initialize Main Path"))
        {
            baseDataPlagin.InitializeMainPath();
            EditorUtility.SetDirty(baseDataPlagin);
        }

        if (GUILayout.Button(baseDataPlagin.developerMode ? "Developer Mode: True" : "Developer Mode: False"))
        {
            baseDataPlagin.developerMode = !baseDataPlagin.developerMode;
        }
    }
} test
*/ 