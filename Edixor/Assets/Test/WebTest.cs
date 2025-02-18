using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MyWebViewWindow : EditorWindow {
    private object _webView;
    private Type _webViewType;
    private string _url = "https://www.google.com";

    [MenuItem("Window/My WebView")]
    public static void ShowWindow() {
        GetWindow<MyWebViewWindow>("My WebView");
    }

    private void OnEnable() {
        // Попытаемся получить тип EditorWebView через сборку UnityEditor,
        // в которой находится класс Editor.
        _webViewType = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.WebView.EditorWebView");
        if (_webViewType == null) {
            Debug.LogError("EditorWebView не найден. Убедитесь, что вы используете правильную версию Unity (6000.0.29f1).");
            return;
        }
        // Создаем экземпляр EditorWebView, передавая текущее окно (this)
        _webView = Activator.CreateInstance(_webViewType, new object[] { this });
        // Загружаем нужный URL через метод LoadURL
        MethodInfo loadURLMethod = _webViewType.GetMethod("LoadURL", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (loadURLMethod != null) {
            loadURLMethod.Invoke(_webView, new object[] { _url });
        }
    }

    private void OnGUI() {
        if (_webView != null) {
            // Вызываем внутренний OnGUI для EditorWebView, чтобы он отрисовался в окне.
            MethodInfo onGUI = _webViewType.GetMethod("OnGUI", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (onGUI != null) {
                onGUI.Invoke(_webView, null);
            }
        } else {
            GUILayout.Label("EditorWebView не доступен");
        }
    }

    private void Update() {
        if (_webView != null) {
            // Вызываем метод Update, если он доступен
            MethodInfo updateMethod = _webViewType.GetMethod("Update", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (updateMethod != null) {
                updateMethod.Invoke(_webView, null);
            }
        }
    }

    private void OnDisable() {
        if (_webView != null) {
            // Вызываем Dispose для очистки ресурсов
            MethodInfo disposeMethod = _webViewType.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (disposeMethod != null) {
                disposeMethod.Invoke(_webView, null);
            }
        }
    }
}
