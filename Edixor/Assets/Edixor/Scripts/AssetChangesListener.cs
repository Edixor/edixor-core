#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetChangesListener : AssetPostprocessor
{
    // Флаг для предотвращения множественного перезапуска
    private static bool isRestartPending = false;

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {
        // Если был изменён хотя бы один файл
        if (importedAssets.Length > 0 || deletedAssets.Length > 0 || movedAssets.Length > 0)
        {
            // Проверяем, существует ли окно
            var window = EdixorWindow.CurrentWindow;
            if (window != null && !isRestartPending)
            {
                isRestartPending = true; // Устанавливаем флаг

                // Ожидаем до следующего кадра перед перезапуском окна
                EditorApplication.delayCall += () =>
                {
                    window.RestartWindow();
                    isRestartPending = false; // Сбрасываем флаг
                };
            }
        }
    }
}
#endif
