using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneUtils
{

    public static bool ThisSceneIsNotGameplayScene(string sceneName)
    {
        if(sceneName == Loader.Scene.UIScene.ToString() ||
            sceneName == Loader.Scene.LoadingScene.ToString() ||
            sceneName == Loader.Scene.MainMenu.ToString() ||
            sceneName == Loader.Scene.CharacterSelectScene.ToString() ||
            sceneName == Loader.Scene.LobbyScene.ToString() ||
            sceneName == Loader.Scene.CutScene.ToString())
            return true;

        return false;
    }

    public static bool ThisSceneIsGameplayScene(string sceneName)
    {
        if (sceneName != Loader.Scene.UIScene.ToString() &&
            sceneName != Loader.Scene.LoadingScene.ToString() &&
            sceneName != Loader.Scene.MainMenu.ToString() &&
            sceneName != Loader.Scene.CharacterSelectScene.ToString() &&
            sceneName != Loader.Scene.LobbyScene.ToString() &&
            sceneName != Loader.Scene.CutScene.ToString())
            return true;

        return false;
    }
    public static bool ThisSceneIsNotWorldScene(string sceneName)
    {
        if (sceneName != Loader.Scene.WorldScene.ToString())
            return true;

        return false;
    }


    public static GameObject FindGameObjectWithTagInScene(string tag, Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            GameObject result = FindTaggedInHierarchy(root.transform, tag);
            if (result != null) return result;
        }

        return null;
    }

    private static GameObject FindTaggedInHierarchy(Transform parent, string tag)
    {
        if (parent.CompareTag(tag))
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject result = FindTaggedInHierarchy(child, tag);
            if (result != null) return result;
        }

        return null;
    }

    public static GameObject FindGameObjectByNameInScene(string name, Scene scene)
    {
        if (!scene.IsValid() || !scene.isLoaded)
            return null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            GameObject result = FindNamedInHierarchy(root.transform, name);
            if (result != null) return result;
        }

        return null;
    }
    public static List<GameObject> FindAllGameObjectsByNameInScene(string name, Scene scene)
    {
        List<GameObject> results = new();

        if (!scene.IsValid() || !scene.isLoaded)
            return results;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            FindAllNamedInHierarchy(root.transform, name, results);
        }

        return results;
    }
    private static void FindAllNamedInHierarchy(Transform parent, string name, List<GameObject> results)
    {
        if (parent.name == name)
            results.Add(parent.gameObject);

        foreach (Transform child in parent)
        {
            FindAllNamedInHierarchy(child, name, results);
        }
    }
    private static GameObject FindNamedInHierarchy(Transform parent, string name)
    {
        if (parent.name == name)
            return parent.gameObject;

        foreach (Transform child in parent)
        {
            GameObject result = FindNamedInHierarchy(child, name);
            if (result != null) return result;
        }

        return null;
    }

    public static List<T> FindAllOfTypeInScene<T>(Scene scene) where T : Component
    {
        var results = new List<T>();

        if (!scene.IsValid() || !scene.isLoaded)
        {
            Debug.LogWarning("Scene is not valid or not loaded.");
            return results;
        }

        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            results.AddRange(rootObj.GetComponentsInChildren<T>(true)); // true = include inactive
        }

        return results;
    }
}

