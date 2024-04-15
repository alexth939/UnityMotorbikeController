using UnityEngine;

namespace FolderIcons
{
    /// <summary>
    /// Usage:
    /// <code>
    /// using(_ = GUIPropertyOverrider.SetIsEnbled(false))
    /// using(_ = GUIPropertyOverrider.SetContentColor(Color.green))
    /// {
    ///     _ = EditorGUILayout.ObjectField("Folder", entry.Folder, typeof(DefaultAsset), isSceneObjectsAlloed) as DefaultAsset;
    /// }
    /// 
    /// Instead of:
    /// 
    /// bool isGUIEnabledCache = GUI.enabled;
    /// Color gUIContentColorCache = GUI.contentColor;
    /// GUI.enabled = false;
    /// GUI.contentColor = Color.green;
    /// _ = EditorGUILayout.ObjectField("Folder", entry.Folder, typeof(DefaultAsset), isSceneObjectsAlloed) as DefaultAsset;
    /// GUI.contentColor = gUIContentColorCache;
    /// GUI.enabled = isGUIEnabledCache;
    /// </code>
    /// </summary>
    public class GUIPropertyOverrider : System.IDisposable
    {
        private System.Action _fallback;

        private GUIPropertyOverrider() { }

        public static GUIPropertyOverrider SetIsEnbled(bool isEnbledFlag)
        {
            var instance = new GUIPropertyOverrider();

            bool initialIsEnbledValue = GUI.enabled;
            GUI.enabled = isEnbledFlag;
            instance._fallback = () => GUI.enabled = initialIsEnbledValue;

            return instance;
        }

        public static GUIPropertyOverrider SetColor(Color color)
        {
            var instance = new GUIPropertyOverrider();

            Color initialColorValue = GUI.color;
            GUI.color = color;
            instance._fallback = () => GUI.color = initialColorValue;

            return instance;
        }

        public static GUIPropertyOverrider SetContentColor(Color color)
        {
            var instance = new GUIPropertyOverrider();

            Color initialContentColorValue = GUI.contentColor;
            GUI.contentColor = color;
            instance._fallback = () => GUI.contentColor = initialContentColorValue;

            return instance;
        }

        public void Dispose() => _fallback.Invoke();
    }
}
