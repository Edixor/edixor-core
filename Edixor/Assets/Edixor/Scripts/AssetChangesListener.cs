#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetChangesListener : AssetPostprocessor
{
    // Статическое событие, которое будет уведомлять о перезапуске окна
    public static event System.Action OnRestartPending;

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
            if (!isRestartPending)
            {
                isRestartPending = true; // Устанавливаем флаг

                // Оповещаем всех подписчиков, что перезапуск запланирова
                OnRestartPending?.Invoke();
            }
        }
    }
}
#endif 
