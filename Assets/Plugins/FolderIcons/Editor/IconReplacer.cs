using UnityEngine;
using UnityEditor;

namespace FolderIcons
{
    [InitializeOnLoad]
    public class IconReplacer
    {
        private FolderIconSettings _settings;

        static IconReplacer()
        {
            _ = new IconReplacer()
            {
                _settings = FolderIconSettings.LoadOrCreateSettings(),
            };
        }

        public IconReplacer()
        {
            FolderIconSettings.SettingsUpdated += ReloadSettingsFile;
            EditorApplication.projectWindowItemOnGUI -= OnDrawingProjectWindowItem;
            EditorApplication.projectWindowItemOnGUI += OnDrawingProjectWindowItem;
        }

        private void ReloadSettingsFile(FolderIconSettings settings)
        {
            _settings = settings;
        }

        private void OnDrawingProjectWindowItem(string guid, Rect selectionRect) => ReplaceIcons(guid, selectionRect);

        private void ReplaceIcons(string guid, Rect selectionRect)
        {
            if(_settings == null)
            {
                return;
            }

            if(!_settings.showCustomFolder)
            {
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            Object folderAsset = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));

            if(folderAsset == null)
            {
                return;
            }

            for(int i = 0; i < _settings.icons.Count; i++)
            {
                FolderIcon icon = _settings.icons[i];

                if(icon.Folder != folderAsset)
                {
                    continue;
                }

                DrawTextures(selectionRect, icon, guid);
            }
        }

        private static void DrawTextures(Rect rect, FolderIcon icon, string guid)
        {
            bool isTreeView = rect.width > rect.height;
            bool isSideView = FolderIconGUI.IsSideView(rect);

            // Vertical Folder View
            if(isTreeView)
            {
                rect.width = rect.height = FolderIconConstants.MAX_TREE_HEIGHT;

                //Add small offset for correct placement
                if(!isSideView)
                {
                    rect.x += 3f;
                }
            }
            else
            {
                rect.height -= 14f;
            }

            if(icon.Icon)
            {
                FolderIconGUI.DrawFolderTexture(rect, icon.Icon);
            }
        }
    }
}
