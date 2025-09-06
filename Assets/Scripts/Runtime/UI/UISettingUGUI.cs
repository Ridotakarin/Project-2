using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISettingUGUI : MonoBehaviour
{
    public TMP_Dropdown screenResolution;
    public Toggle fullScreen;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        screenResolution.value = SettingsManager.Instance.defaultSettings.settingsData.ResolutionIndex;
        fullScreen.isOn = SettingsManager.Instance.defaultSettings.settingsData.IsFullScreen;

        masterSlider.value = AudioManager.Instance.GetVolume("MasterVolume");
        musicSlider.value = AudioManager.Instance.GetVolume("MusicVolume");
        sfxSlider.value = AudioManager.Instance.GetVolume("SFXVolume");

        screenResolution.onValueChanged.AddListener(SettingsManager.Instance.SetResolution);
        fullScreen.onValueChanged.AddListener(SettingsManager.Instance.SetFullScreen);

        masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}
