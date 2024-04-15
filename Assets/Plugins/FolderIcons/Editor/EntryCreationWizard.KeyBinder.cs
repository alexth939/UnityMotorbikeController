using UnityEditor;
using UnityEngine;

namespace FolderIcons
{
    public partial class EntryCreationWizard
    {
        [InitializeOnLoadMethod]
        private static void BindEntryCreationWizardService()
        {
            EditorApplication.projectWindowItemOnGUI -= OnDrawingProjectWindowItem;
            EditorApplication.projectWindowItemOnGUI += OnDrawingProjectWindowItem;
        }

        private static void OnDrawingProjectWindowItem(string guid, Rect selectionRect)
        {
            if(Event.current != null &&
                Event.current.alt &&
                Event.current.type == EventType.MouseDown &&
                selectionRect.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if(IsItAFolder(assetPath))
                    OpenDialogue(assetPath);
            }
        }

        private static bool IsItAFolder(string assetPath) => AssetDatabase.IsValidFolder(assetPath);
    }
}
