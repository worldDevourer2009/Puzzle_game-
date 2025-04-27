using UnityEditor;

namespace Editor
{
    public static class AssetsImporterHelper
    {
        private static EditorWindow _window;

        [MenuItem("AssetsImporterHelper/Manage Import Settings")]
        private static void ManageImportSettings()
        {
            _window = EditorWindow.GetWindow<AssetImporterWindow>();
            _window.Show();
        }
    }
}