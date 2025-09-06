using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneManagement
{
    public static string SceneName { get; private set; }
    public static string SceneTransitionName { get; private set; }

    public static string GetCurrentSceneName()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public static void SetTransitionName(string transitionName)
    {
        SceneTransitionName = transitionName;
    }
}