using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FolderIcons
{
    public class FolderIconSettings : ScriptableObject
    {
        public List<FolderIcon> icons = new();
        public bool showCustomFolder = true;

        internal static event Action<FolderIconSettings> SettingsUpdated;

        private static string ProjectSettingsFolderPath => Application.dataPath.Replace("Assets", "ProjectSettings");

        private static string SettingsFilePath => Path.Combine(ProjectSettingsFolderPath, "FolderIconSettings.asset");

        internal static void CreateOrOverwriteEntry(FolderIcon entry)
        {
            FolderIconSettings settings = LoadOrCreateSettings();
            int existingEntryIndex = settings.icons.FindIndex(existingEntry => existingEntry.Folder == entry.Folder);

            if(existingEntryIndex == -1)
            {
                settings.icons.Add(entry);
            }
            else
            {
                settings.icons[existingEntryIndex] = entry;
            }

            SaveSettings(settings);
        }

        internal static FolderIconSettings LoadOrCreateSettings()
        {
            if(TryLoadSettings(out FolderIconSettings settings) == false)
            {
                settings = CreateInstance<FolderIconSettings>();
                SaveSettings(settings);
            }

            return settings;
        }

        internal static void SaveSettings(FolderIconSettings settings)
        {
            var wrappedSettings = new Object[] { settings };
            bool isTextSerializationAlloed = true;
            InternalEditorUtility.SaveToSerializedFileAndForget(wrappedSettings, SettingsFilePath, isTextSerializationAlloed);
            SettingsUpdated?.Invoke(settings);
        }

        internal static bool TryLoadSettings(out FolderIconSettings settings)
        {
            Object[] loadedSomething = InternalEditorUtility.LoadSerializedFileAndForget(SettingsFilePath);
            settings = loadedSomething.SingleOrDefault() as FolderIconSettings;

            return settings != null;
        }

        internal static bool TryRemoveEntry(FolderIcon _entry)
        {
            FolderIconSettings settings = LoadOrCreateSettings();
            int existingEntryIndex = settings.icons.FindIndex(entry => entry.Folder == _entry.Folder);

            if(existingEntryIndex != -1)
            {
                settings.icons.RemoveAt(existingEntryIndex);
                SaveSettings(settings);
            }

            return existingEntryIndex != -1;
        }
    }
}
