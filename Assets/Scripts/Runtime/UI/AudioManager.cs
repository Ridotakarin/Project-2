using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AudioManager : PersistentSingleton<AudioManager>
{

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;
    public string masterVolumeParam = "MasterVolume";
    public string musicVolumeParam = "MusicVolume";
    public string sfxVolumeParam = "SFXVolume";

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public List<NamedAudioClip> musicClips;

    [Header("SFX Clips")]
    public List<NamedAudioClip> sfxClips;

    private Dictionary<string, AudioClip> musicDict;
    private Dictionary<string, AudioClip> sfxDict;

    protected override void Awake()
    {
        base.Awake();
        // Convert lists to dictionaries for fast lookup
        musicDict = new Dictionary<string, AudioClip>();
        foreach (var item in musicClips)
            musicDict[item.name] = item.clip;

        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var item in sfxClips)
            sfxDict[item.name] = item.clip;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "MainMenu":
                PlayMusic("Chill");
                break;
            case "WorldScene":
                PlayMusic("Default");
                break;
            case "MineCaveScene":
                PlayMusic("Cave");
                break;
            case "MineScene":
                PlayMusic("Cave");
                break;
            case "LoadingScene":
                StopMusic();
                break;
            default:
                break;
        }
    }

    public void PlayMusic(string name)
    {
        if (musicDict.TryGetValue(name, out var clip))
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip '{name}' not found!");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string name)
    {
        if (sfxDict.TryGetValue(name, out var clip))
        {
            sfxSource.pitch = Random.Range(0.8f, 1.2f); 
            sfxSource.PlayOneShot(clip);

            //sfxSource.pitch = originalPitch; // reset pitch too soon so it doesn't affected
        }
        else
        {
            Debug.LogWarning($"SFX clip '{name}' not found!");
        }
    }
    public void PlaySFX(AudioClip audioClip)
    {
        if (audioClip != null)
        {
            sfxSource.pitch = Random.Range(0.8f, 1.2f); // Random pitch variation
            sfxSource.PlayOneShot(audioClip);
        }
        else
        {
            Debug.LogWarning("SFX clip is null!");
        }
    }


    public void SetMasterVolume(float volume) => SetVolume(masterVolumeParam, volume);
    public void SetMusicVolume(float volume) => SetVolume(musicVolumeParam, volume);
    public void SetSFXVolume(float volume) => SetVolume(sfxVolumeParam, volume);

    private void SetVolume(string param, float volume)
    {
        //float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(param, volume);
    }

    public float GetVolume(string param)
    {
        if (audioMixer.GetFloat(param, out float dB))
            return Mathf.Pow(10f, dB / 20f);
        return 1f;
    }
}

[System.Serializable]
public class NamedAudioClip
{
    public string name;
    public AudioClip clip;
}
