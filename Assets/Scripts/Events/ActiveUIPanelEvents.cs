using System;
using UnityEngine;

public class ActiveUIPanelEvents
{
    public event Action onActiveMainMenu;
    public void OnActiveMainMenu()
    {
        onActiveMainMenu?.Invoke();
    }

    public event Action onDisActivateMainMenu;
    public void OnDisActivateMainMenu()
    {
        onDisActivateMainMenu?.Invoke();
    }

    public event Action onActiveSingleplayer;
    public void OnActiveSingleplayer()
    {
        onActiveSingleplayer?.Invoke();
    }

    public event Action onDisActivateSingleplayer;
    public void OnDisActivateSingleplayer()
    {
        onDisActivateSingleplayer?.Invoke();
    }

    public event Action onActiveOptions;
    public void OnActiveOptions()
    {
        onActiveOptions?.Invoke();
    }

    public event Action onDisActivateOptions;
    public void OnDisActivateOptions()
    {
        onDisActivateOptions?.Invoke();
    }

    public event Action onActiveCredits;
    public void OnActiveCredits()
    {
        onActiveCredits?.Invoke();
    }

    public event Action onDisActivateCredits;
    public void OnDisActivateCredits()
    {
        onDisActivateCredits?.Invoke();
    }
}
