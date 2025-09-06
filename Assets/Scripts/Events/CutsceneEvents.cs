using System;
using UnityEngine;

public class CutsceneEvents
{
    public event Action onPlayCutscene;
    public void PlayCutscene()
    {
        onPlayCutscene?.Invoke();
    }

    public event Action onStopCutscene;
    public void StopCutscene()
    {
        onStopCutscene?.Invoke();
    }

    public event Action onPauseCutscene;
    public void PauseCutscene()
    {
        onPauseCutscene?.Invoke();
    }

    public event Action onResumeCutscene;
    public void ResumeCutscene()
    {
        onResumeCutscene?.Invoke();
    }
}
