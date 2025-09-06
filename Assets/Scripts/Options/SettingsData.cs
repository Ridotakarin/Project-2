using UnityEngine;

[System.Serializable]
public class SettingsData
{
    [SerializeField] private float overalVolume;
    [SerializeField] private float musicVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private int resolutionIndex;
    [SerializeField] private bool isFullScreen;

    public float OveralVolume
    {
        get => overalVolume;
        private set => overalVolume = value;
    }

    public float MusicVolume
    {
        get => musicVolume;
        private set => musicVolume = value;
    }

    public float SFXVolume
    {
        get => sfxVolume;
        private set => sfxVolume = value;
    }

    public int ResolutionIndex
    {
        get => resolutionIndex;
        private set => resolutionIndex = value;
    }

    public bool IsFullScreen
    {
        get => isFullScreen;
        private set => isFullScreen = value;
    }

    public SettingsData()
    {
        overalVolume = 20f;
        musicVolume = 20f;
        sfxVolume = 20f;
        resolutionIndex = 0;
        isFullScreen = true;
    }

    public SettingsData(float overalVolume, float musicVolume, float sfxVolume, int resolutionIndex, bool isFullScreen)
    {
        OveralVolume = overalVolume;
        MusicVolume = musicVolume;
        SFXVolume = sfxVolume;
        ResolutionIndex = resolutionIndex;
        IsFullScreen = isFullScreen;
    }

    public void SetOveralVolume(float overalVolume)
    {
        OveralVolume = overalVolume;
    }

    public void SetMusicVolume(float musicVolume)
    {
        MusicVolume = musicVolume;
    }

    public void SetSFXVolume(float sfxVolume)
    { 
        SFXVolume = sfxVolume;
    }

    public void SetResolutionIndex(int resolutionIndex)
    {
        this.resolutionIndex = resolutionIndex;
    }

    public void SetIsFullScreen(bool isFullScreen)
    { 
        this.isFullScreen = isFullScreen; 
    }
}
