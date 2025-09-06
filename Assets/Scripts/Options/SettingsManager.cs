using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : PersistentSingleton<SettingsManager>
{
    [Header("Reference")]
    public DefaultSettingsSO defaultSettings;

    [Header("Audio")]
    public AudioMixer audioMixer;

    private SettingsData settingsData;

    private void Start()
    {
        LoadData();
    }

    public void ApplySettings()
    {
        SetMasterVolume(defaultSettings.settingsData.OveralVolume);
        SetMusicVolume(defaultSettings.settingsData.MusicVolume);
        SetSFXVolume(defaultSettings.settingsData.SFXVolume);
        SetResolution(defaultSettings.settingsData.ResolutionIndex);
        SetFullScreen(defaultSettings.settingsData.IsFullScreen);
    }

    public void SetMasterVolume(float volume)
    {
        defaultSettings.settingsData.SetOveralVolume(volume);
        SetVolumeInMixer("MasterVolume", volume);
    }

    public void SetMusicVolume(float volume)
    {
        defaultSettings.settingsData.SetMusicVolume(volume);
        SetVolumeInMixer("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        defaultSettings.settingsData.SetSFXVolume(volume);
        SetVolumeInMixer("SFXVolume", volume);
    }

    private void SetVolumeInMixer(string parameter, float volume)
    {
        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }


    public void SetResolution(int resolutionIndex)
    {
        defaultSettings.settingsData.SetResolutionIndex(resolutionIndex);

        switch (resolutionIndex)
        {
            case 0:
                Screen.SetResolution(1920, 1080, defaultSettings.settingsData.IsFullScreen);
                break;
            case 1:
                Screen.SetResolution(1280, 720, defaultSettings.settingsData.IsFullScreen);
                break;
            case 2:
                Screen.SetResolution(800, 600, defaultSettings.settingsData.IsFullScreen);
                break;
            default:
                Debug.LogWarning("Invalid resolution index");
                break;
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        defaultSettings.settingsData.SetIsFullScreen(isFullScreen);
        Screen.fullScreen = defaultSettings.settingsData.IsFullScreen;
    }

    public void LoadData()
    {
        settingsData = SettingsFileHandler.Load(defaultSettings);
        defaultSettings.settingsData = settingsData;
        ApplySettings();
    }

    public void SaveData()
    {
        SettingsFileHandler.Save(defaultSettings.settingsData);
        defaultSettings.ResetSettingData();
    }
}
