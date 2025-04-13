#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class AssetChangesListener : AssetPostprocessor
{

    public static event System.Action OnRestartPending;


    private static bool isRestartPending = false;

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
    {

        if (importedAssets.Length > 0 || deletedAssets.Length > 0 || movedAssets.Length > 0)
        {
            if (!isRestartPending)
            {
                isRestartPending = true;


                OnRestartPending?.Invoke();
            }
        }
    }
}
#endif 
