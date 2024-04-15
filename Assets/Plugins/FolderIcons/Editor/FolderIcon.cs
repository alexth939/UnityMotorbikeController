using System;
using UnityEditor;
using UnityEngine;

namespace FolderIcons
{
    [Serializable]
    public class FolderIcon
    {
        [SerializeField] private DefaultAsset _folder;
        [SerializeField] private Texture2D _icon;

        public FolderIcon(DefaultAsset folder)
        {
            _folder = folder;
        }

        public DefaultAsset Folder => _folder;

        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }

        public static string FolderFieldName => nameof(_folder);
        public static string IconFieldName => nameof(_icon);
    }
}
