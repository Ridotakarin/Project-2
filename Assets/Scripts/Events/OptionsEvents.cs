using System;
using UnityEngine;

public class OptionsEvents
{
    public event Action<float> onScreenResolutionChange;
    public void ScreenResolutionChange(float resolution)
    {
        if (onScreenResolutionChange != null)
        {
            onScreenResolutionChange(resolution);
        }
    }

    public event Action<bool> onFullScreenChange;
    public void FullScreenChange(bool isFullScreen)
    {
        if (onFullScreenChange != null)
        {
            onFullScreenChange(isFullScreen);
        }
    }

    public event Action<bool> onShadowChange;
    public void ShadowChange(bool isShadow)
    {
        if (onShadowChange != null)
        {
            onShadowChange(isShadow);
        }
    }

    public event Action<float> onOveralSoundChange;
    public void OveralSoundChange(float sound)
    {
        if (onOveralSoundChange != null)
        {
            onOveralSoundChange(sound);
        }
    }

    public event Action<string> onSoundEffectChange; 
    public void SoundEffectChange(string soundEffect)
    {
        if (onSoundEffectChange != null)
        {
            onSoundEffectChange(soundEffect);
        }
    }

    public event Action<string> onMusicChange;
    public void MusicChange(string music)
    {
        if (onMusicChange != null)
        {
            onMusicChange(music);
        }
    }

    public event Action<float> onMouseSpeedChange;
    public void MouseSpeedChange(float speed)
    {
        if (onMouseSpeedChange != null)
        {
            onMouseSpeedChange(speed);
        }
    }

    public event Action<string, string> onHotKeyChange;
    public void HotKeyChange(string action, string key)
    {
        if (onHotKeyChange != null)
        {
            onHotKeyChange(action, key);
        }
    }
}
