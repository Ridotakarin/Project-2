using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;

public class MultiSceneManger : PersistentSingleton<MultiSceneManger>
{
    private Scene WorldScene;
    public Scene ActiveSubScene;
    [property: SerializeField]
    public string ActiveSubSceneName => ActiveSubScene.name;

    private List<AreaEntrance> allEntranceInWorldScene = new List<AreaEntrance>();

    private GameObject _worldSceneMainCamera;
    private GameObject _worldSceneVirtualCamera;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void SetGlobalLightActiveInScene(Scene scene, bool active)
    {
        if (!scene.IsValid() || !scene.isLoaded) return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            var globalLights = root.GetComponentsInChildren<UnityEngine.Rendering.Universal.Light2D>(true);
            foreach (var light in globalLights)
            {
                if (light.lightType == UnityEngine.Rendering.Universal.Light2D.LightType.Global)
                {
                    light.gameObject.SetActive(active);
                }
            }
        }
    }

    public void OnExitToWorldScene(Scene SceneToLoad)
    {
        SetWorldSceneCameraStatus(true);
        FindEntranceToSpawn();
        SetGlobalLightActiveInScene(WorldScene, true);

        CameraController.Instance.RefreshFollowCamera(_worldSceneVirtualCamera.GetComponent<CinemachineVirtualCamera>());
        GameEventsManager.Instance.objectEvents.SpawnObject();
        GameEventsManager.Instance.dataEvents.OnExitToWorldScene(SceneToLoad.name);
        AudioManager.Instance.PlayMusic("Default");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        if (scene.name == Loader.Scene.MainMenu.ToString() ||
            scene.name == Loader.Scene.CutScene.ToString() ||
            scene.name == Loader.Scene.CharacterSelectScene.ToString() ||
            scene.name == Loader.Scene.LobbyScene.ToString() ||
            scene.name == Loader.Scene.UIScene.ToString() ||
            scene.name == Loader.Scene.LoadingScene.ToString())
            return;

        if (scene.name == Loader.Scene.WorldScene.ToString())
        {
            GameObject mainWorldSceneCamera = SceneUtils.FindGameObjectWithTagInScene("MainCamera", scene);
            GameObject worldSceneVirtualCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", scene);
            
            SetWorldSceneCameraObject(mainWorldSceneCamera, worldSceneVirtualCamera);
            SaveWorldSceneRef(scene);

            Debug.Log("World scene references saved after load.");
        }
        else // load to subscene
        {
            Debug.Log("Did OnSwitchSceneWhileGameplay");
            SetWorldSceneCameraStatus(false);
            SetGlobalLightActiveInScene(WorldScene, false);
            var newFollowCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera", scene).GetComponent<CinemachineVirtualCamera>();
            CameraController.Instance.RefreshFollowCamera(newFollowCamera);
        }

        FindAllNetworkSingletonAndSpawnOnSceneLoad(scene);
    }

    public void FindAllNetworkSingletonAndSpawnOnSceneLoad(Scene scene)
    {
        Debug.Log("scene loaded name to spawn object is: " + scene.name);
        List<NetworkObject> netObjs = SceneUtils.FindAllOfTypeInScene<NetworkObject>(scene);

        foreach (var netObj in netObjs)
        {
            if(!netObj.IsSpawned)
                netObj.Spawn();
        }

        if(!TileManager.Instance.IsSpawned)
        TileManager.Instance.GetComponent<NetworkObject>().Spawn();
        if(!CropManager.Instance.IsSpawned)
        CropManager.Instance.GetComponent<NetworkObject>().Spawn();
        if(!EnviromentalStatusManager.Instance.IsSpawned)
            EnviromentalStatusManager.Instance.GetComponent<NetworkObject>().Spawn();

        GameEventsManager.Instance.networkObjectEvents.OnNetworkObjectSpawned();
    }


    #region WorldScene Gameobject Referrences
    public void SetWorldSceneCameraObject(GameObject cameraObject, GameObject virtualCameraObject)
    {
        _worldSceneMainCamera = cameraObject;
        _worldSceneVirtualCamera = virtualCameraObject;
    }

    public void SaveWorldSceneRef(Scene worldScene)
    {
        WorldScene = worldScene;
    }

    public void SetWorldSceneCameraStatus(bool active)
    {
        if(_worldSceneMainCamera != null)
        _worldSceneMainCamera.SetActive(active);
        if(_worldSceneVirtualCamera != null)
        _worldSceneVirtualCamera.SetActive(active);
    }
    #endregion

    #region Entrances
    public void SubscribeToEntranceList(AreaEntrance entrance)
    {
        allEntranceInWorldScene.Add(entrance);
    }

    public void UnsubscribeFromEntranceList(AreaEntrance entrance)
    {
        allEntranceInWorldScene.Remove(entrance);
    }

    private void FindEntranceToSpawn()
    {
        if(allEntranceInWorldScene == null)
        {
            Debug.Log("No entrance registered");
            return;
        }
        foreach (AreaEntrance entrance in allEntranceInWorldScene)
        {
            if (entrance.CheckAndSpawnPlayer())
                return;
        }
    }
    #endregion
}
