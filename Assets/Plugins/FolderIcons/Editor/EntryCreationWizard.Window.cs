using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FolderIcons
{
    public partial class EntryCreationWizard : EditorWindow
    {
        private const string TitleText = "Entry Creation Wizard";

        private FolderIcon _entry;

        public static void DrawFolderPreview(Rect rect, Texture folder)
        {
            if(folder == null)
            {
                return;
            }

            if(folder != null)
            {
                GUI.DrawTexture(rect, folder, ScaleMode.ScaleToFit);
            }

            //Half size of overlay, and reposition to center
            rect.size *= 0.5f;
            rect.position += rect.size * 0.5f;
        }

        public static void OpenDialogue(string assetPath)
        {
            bool isWindowOfUtilityType = true;
            var window = GetWindow<EntryCreationWizard>(isWindowOfUtilityType);
            window.titleContent = new GUIContent(TitleText);

            var folderAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(DefaultAsset)) as DefaultAsset;
            FolderIconSettings settings = FolderIconSettings.LoadOrCreateSettings();
            FolderIcon folderIconEntry = settings.icons.FirstOrDefault(entry => entry.Folder == folderAsset);

            window.position = new Rect()
            {
                x = Screen.currentResolution.width / 2 - 300,
                y = Screen.currentResolution.height / 2 - 300,
                width = 300,
                height = 300,
            };

            if(folderIconEntry == null)
            {
                window._entry = new FolderIcon(folderAsset);
            }
            else
            {
                window._entry = folderIconEntry;
            }

            bool isImmediateDisplayRequired = false;
            window.Show(isImmediateDisplayRequired);
        }

        // It was a great idea, but unfortunately it causes bugs when we loose focus when chosing a texture.
        //private void OnLostFocus()
        //{
        //    Close();
        //}

        private void CloseOnEscapePressed()
        {
            if(Event.current.keyCode == KeyCode.Escape)
                Close();
        }

        private void DrawEntryAndAssignChanges(FolderIcon entry)
        {
            bool isSceneObjectsAlloed = false;

            using(_ = GUIPropertyOverrider.SetIsEnbled(false))
            {
                _ = EditorGUILayout.ObjectField("Folder", entry.Folder, typeof(DefaultAsset), isSceneObjectsAlloed) as DefaultAsset;
            }

            entry.Icon = EditorGUILayout.ObjectField("Icon", entry.Icon, typeof(Texture2D), isSceneObjectsAlloed) as Texture2D;
        }

        private void OnGUI()
        {
            var position = EditorGUILayout.BeginVertical();
            DrawEntryAndAssignChanges(_entry);
            EditorGUILayout.EndVertical();

            position.y += position.height;

            DrawFolderPreview(position, _entry.Icon);

            GUILayout.BeginVertical(GUILayout.Height(position.height), GUILayout.ExpandHeight(true));
            GUILayout.EndVertical();

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("Apply"))
            {
                FolderIconSettings.CreateOrOverwriteEntry(_entry);
                Close();
                FolderIconSettingsEditor.RepaintProjectSettingsWindow();
            }

            if(GUILayout.Button("Clear"))
            {
                //RemoveEntry();
                _ = FolderIconSettings.TryRemoveEntry(_entry);
                Close();
                FolderIconSettingsEditor.RepaintProjectSettingsWindow();
            }

            GUILayout.EndHorizontal();

            CloseOnEscapePressed();
        }
    }
}
