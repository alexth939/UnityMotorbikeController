using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace FolderIcons
{
    public partial class FolderIconSettingsEditor : SettingsProvider
    {
        private const string EntryPath = "Plugins/FolderIcons";

        private ReorderableList _iconList;
        private GUIStyle _previewStyle;
        private SerializedProperty _serializedIcons;
        private SerializedObject _serializedObject;
        private FolderIconSettings _settings;
        private bool _showCustomFolders;

        public FolderIconSettingsEditor(string path, SettingsScope scopes = SettingsScope.Project) : base(path, scopes)
        {
        }

        // Use it for entry initialization. It is called when u open "Edit/Project Settings".
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var keywordsForSearch = new string[]
            {
                "FolderIcons",
                "Window",
                "Project Window",
                "Colorize",
                "Icons"
            };

            var provider = new FolderIconSettingsEditor(EntryPath, SettingsScope.Project)
            {
                keywords = keywordsForSearch,
                label = "Folder Icons",
            };

            return provider;
        }

        public static void RepaintProjectSettingsWindow()
        {
            if(TryFindOpenedProjectSettingsWindow(out EditorWindow projectSettingsWindow))
                projectSettingsWindow.Repaint();
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _settings = FolderIconSettings.LoadOrCreateSettings();
            InitStyles();

            _showCustomFolders = _settings.showCustomFolder;
            _serializedObject = new SerializedObject(_settings);
            _serializedIcons = _serializedObject.FindProperty("icons");

            if(_iconList == null)
            {
                _iconList = new ReorderableList(_serializedObject, _serializedIcons)
                {
                    drawHeaderCallback = OnHeaderDraw,

                    drawElementCallback = OnElementDraw,
                    drawElementBackgroundCallback = DrawElementBackground,

                    elementHeightCallback = GetPropertyHeight,

                    showDefaultBackground = false,
                };
            }

            FolderIconSettings.SettingsUpdated -= ReloadSettings;
            FolderIconSettings.SettingsUpdated += ReloadSettings;
        }

        private static bool TryFindOpenedProjectSettingsWindow(out EditorWindow window)
        {
            const string SettingsWindowTypeName = "UnityEditor.ProjectSettingsWindow";
            EditorWindow[] openedEditorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>();

            foreach(var openedWindow in openedEditorWindows)
            {
                if(openedWindow.GetType().ToString() == SettingsWindowTypeName)
                {
                    window = openedWindow;
                    return true;
                }
            }

            window = null;
            return false;
        }

        private void ReloadSettings(FolderIconSettings settings)
        {
            _settings = settings;
            _showCustomFolders = _settings.showCustomFolder;
            _serializedObject = new SerializedObject(_settings);
            _serializedIcons = _serializedObject.FindProperty("icons");

            _iconList = new ReorderableList(_serializedObject, _serializedIcons)
            {
                drawHeaderCallback = OnHeaderDraw,

                drawElementCallback = OnElementDraw,
                drawElementBackgroundCallback = DrawElementBackground,

                elementHeightCallback = GetPropertyHeight,

                showDefaultBackground = false,
            };
        }
    }
}
