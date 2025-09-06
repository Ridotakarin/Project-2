using System.IO;
using UnityEngine;

public static class SettingsFileHandler
{
    private static string FilePath => Application.persistentDataPath + "/settings.json";

    public static void Save(SettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    public static SettingsData Load(DefaultSettingsSO defaultSO)
    {
        if (!File.Exists(FilePath))
        {
            return new SettingsData(defaultSO.settingsData.OveralVolume, 
                                    defaultSO.settingsData.MusicVolume, 
                                    defaultSO.settingsData.SFXVolume, 
                                    defaultSO.settingsData.ResolutionIndex, 
                                    defaultSO.settingsData.IsFullScreen);
        }

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<SettingsData>(json);
    }
}
