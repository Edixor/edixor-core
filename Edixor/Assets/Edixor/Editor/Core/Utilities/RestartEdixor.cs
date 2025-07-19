using UnityEditor;

namespace ExTools.Menu
{
    public static class RestartEdixor
    {
        [MenuItem("Edixor/Restart")]
        public static void RestartInit()
        {
            EdixorEntryPoint.Reinitialize();
        }
    }
}
