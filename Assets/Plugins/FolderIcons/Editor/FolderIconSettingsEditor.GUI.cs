using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FolderIcons
{
    public partial class FolderIconSettingsEditor : SettingsProvider
    {
        private const float MAX_FIELD_WIDTH = 150f;
        private const float MAX_LABEL_WIDTH = 90f;
        private const float PROPERTY_HEIGHT = 19f;
        private const float PROPERTY_PADDING = 4f;

        private GUIStyle _elementStyle;
        private int _heightIndex;

        public override void OnGUI(string searchContext)
        {
            EditorGUI.BeginChangeCheck();
            {
                _showCustomFolders = EditorGUILayout.ToggleLeft("Show Folder Textures", _showCustomFolders);
            }

            if(EditorGUI.EndChangeCheck())
            {
            }

            EditorGUILayout.Space(16f);

            EditorGUI.BeginChangeCheck();
            _iconList.DoLayoutList();

            if(EditorGUI.EndChangeCheck())
            {
                _serializedObject.ApplyModifiedProperties();
                FolderIconSettings.SaveSettings(_serializedObject.targetObject as FolderIconSettings);
            }
        }

        private void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.LabelField(rect, "", _elementStyle);

            Color fill = (isFocused) ? FolderIconConstants.SelectedColor : Color.clear;

            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(rect, fill, new Color(0.15f, 0.15f, 0.15f, 1f));
            Handles.EndGUI();
        }

        private void DrawPropertyNoDepth(Rect rect, SerializedProperty property)
        {
            rect.width++;
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(rect, Color.clear, new Color(0.15f, 0.15f, 0.15f, 1f));
            Handles.EndGUI();

            rect.x++;
            rect.width -= 3;
            rect.y += PROPERTY_PADDING;
            rect.height = PROPERTY_HEIGHT;

            SerializedProperty copy = property.Copy();
            bool enterChildren = true;

            while(copy.Next(enterChildren))
            {
                if(SerializedProperty.EqualContents(copy, property.GetEndProperty()))
                {
                    break;
                }

                EditorGUI.PropertyField(rect, copy, false);
                rect.y += PROPERTY_HEIGHT + PROPERTY_PADDING;

                enterChildren = false;
            }
        }

        private float GetPropertyHeight(int index)
        {
            return GetPropertyHeight(_serializedIcons.GetArrayElementAtIndex(index));
        }

        private float GetPropertyHeight(SerializedProperty property)
        {
            if(_heightIndex == 0)
            {
                _heightIndex = property.CountInProperty();
            }

            //Property count returning wrong, so just supplying 3 for now
            //TODO: Investigate GetPropertyCount and find issue with invalid value
            return (PROPERTY_HEIGHT + PROPERTY_PADDING) * (3) + PROPERTY_PADDING;
        }

        private void InitStyles()
        {
            if(_previewStyle == null)
            {
                _previewStyle = new GUIStyle(EditorStyles.label)
                {
                    fixedHeight = 64,
                    alignment = TextAnchor.MiddleCenter
                };

                _elementStyle = new GUIStyle(EditorStyles.helpBox);// GUI.skin.box)
            }
        }

        private void OnElementDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            if(index >= _serializedIcons.arraySize)
                return;

            SerializedProperty property = _serializedIcons.GetArrayElementAtIndex(index);
            float fullWidth = rect.width;

            // Set sizes for correct draw
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            float rectWidth = MAX_LABEL_WIDTH + MAX_FIELD_WIDTH;
            EditorGUIUtility.labelWidth = Mathf.Min(EditorGUIUtility.labelWidth, MAX_LABEL_WIDTH);
            rect.width = Mathf.Min(rect.width, rectWidth);

            //Draw property and settings in a line
            DrawPropertyNoDepth(rect, property);

            // ==========================
            //     Draw Icon Example
            // ==========================
            rect.x += rect.width;
            rect.width = fullWidth - rect.width;

            // References
            SerializedProperty folderTexture = property.FindPropertyRelative(FolderIcon.IconFieldName);

            // Object checks
            Object folderObject = folderTexture.objectReferenceValue;

            FolderIconGUI.DrawFolderPreview(rect, folderObject as Texture);

            // Revert width modification
            EditorGUIUtility.labelWidth = originalLabelWidth;
        }

        private void OnHeaderDraw(Rect rect)
        {
            rect.y += 5f;
            rect.x -= 6f;
            rect.width += 12f;

            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(rect,
                new Color(0.15f, 0.15f, 0.15f, 1f),
                new Color(0.15f, 0.15f, 0.15f, 1f));
            Handles.EndGUI();

            EditorGUI.LabelField(rect, "Folders", EditorStyles.boldLabel);
        }
    }
}
