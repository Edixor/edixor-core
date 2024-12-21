#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetChangesListener : AssetPostprocessor
{
    // Этот метод сработает при изменении любого файла в проекте
    static void OnPostprocessAllAssets(
        string[] importedAssets, 
        string[] deletedAssets, 
        string[] movedAssets, 
        string[] movedFromAssetPaths)
    {
        // Если любой файл был изменен, перезапускаем окно
        if (importedAssets.Length > 0 || deletedAssets.Length > 0 || movedAssets.Length > 0)
        {
            Debug.Log("Assets changed, checking window state...");

            // Проверяем, существует ли окно и не закрыто ли оно
            var window = EdixorWindow.currentWindow;
            if (window != null)
            {
                // Если окно открыто, то перезапустить его
                window.RestartWindow();
            }
            else
            {
                Debug.Log("Window is not open, skipping restart.");
            }
        }
    }
}
#endif
