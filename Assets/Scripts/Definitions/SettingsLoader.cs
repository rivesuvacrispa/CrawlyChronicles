using System.IO;
using UnityEngine;

namespace Definitions
{
    public class SettingsLoader : MonoBehaviour
    {
        private static string dataPath;
        private static string settingsDir;
        
        public static SettingsData CurrentSettings { get; private set; }
        
        private void Awake()
        {
            dataPath = Application.persistentDataPath;
            settingsDir = dataPath + "/Settings/";

            if (CreateDirectoryIfNotExists(settingsDir))
                LoadSettings();
            else
                SaveSettings(SettingsData.Default());
        }

        private static void LoadSettings()
        {
            string json = File.ReadAllText(settingsDir + "/config.json");
            CurrentSettings = JsonUtility.FromJson<SettingsData>(json);
        }

        public static void SaveSettings(SettingsData settingsData)
        {
            CurrentSettings = settingsData;
            string json = JsonUtility.ToJson(settingsData);
            File.WriteAllText(settingsDir + "/config.json", json);
        }
        
        private static bool CreateDirectoryIfNotExists(string directory)
        {
            bool exists = Directory.Exists(directory) &&
                          File.Exists(directory + "/config.json");
            if (!exists) Directory.CreateDirectory(directory);
            return exists;
        }
    }
}