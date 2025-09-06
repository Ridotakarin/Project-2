using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        WorldScene,
        UIScene,
        MineCaveScene,
        MineScene,
        CutScene,
        LobbyScene,
        LoadingScene,
        CharacterSelectScene,
        HouseScene
    }

    private static Scene targetScene;

    public static Scene TargetScene => targetScene;

    public static bool isMultiSceneLoad = false;

    public static void Load(Scene targetScene, bool isMultiSceneLoad = false)
    {
        Loader.targetScene = targetScene;
        Loader.isMultiSceneLoad = isMultiSceneLoad;
        if(!isMultiSceneLoad)
            SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
        else
            SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString(),LoadSceneMode.Additive);
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        GameEventsManager.Instance.dataEvents.OnDataLoading(targetScene.ToString());
    }

}