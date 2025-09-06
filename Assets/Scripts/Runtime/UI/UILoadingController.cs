using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using Unity.Netcode;
using Cinemachine;

public class UILoadingController : MonoBehaviour
{
    private VisualElement _loadingScreen;
    private VisualElement _progressFill;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        
        _loadingScreen = root.Q<VisualElement>("LoadingScreen");
        _progressFill = root.Q<VisualElement>("ProgressFill");
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.dataEvents.onDataLoading += LoadSceneWithLoading;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.dataEvents.onDataLoading -= LoadSceneWithLoading;
    }

    private void Start()
    {
        Loader.LoaderCallback();
    }

    private void LoadSceneWithLoading(string sceneName)
    {
        _progressFill.style.width = Length.Percent(0f);
        if(!Loader.isMultiSceneLoad)
            StartCoroutine(LoadAsync(sceneName));
        else
            StartCoroutine(MultiSceneLoadAsync(sceneName));
    }

    // this is for While gameplay load scene
    private IEnumerator MultiSceneLoadAsync(string sceneName)
    {
        Debug.Log("run load multi scene");
        // this is for gameplay load to another gameplay scene
        AsyncOperation operation;

        if (sceneName == Loader.Scene.WorldScene.ToString())
        {
            Scene worldScene = SceneManager.GetSceneByName(sceneName);

            if (worldScene.isLoaded)
            {
                Debug.Log("go back to world scene while world scene is loaded");
                operation = SceneManager.UnloadSceneAsync(MultiSceneManger.Instance.ActiveSubScene);
            }
            else
            {
                // load world scene
                operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                // then unload the current subscene
                Debug.Log("current subscene is: " + MultiSceneManger.Instance.ActiveSubSceneName);
                if (MultiSceneManger.Instance.ActiveSubScene.isLoaded)
                    SceneManager.UnloadSceneAsync(MultiSceneManger.Instance.ActiveSubScene);

            }
                
        }
        else
        {
            if(MultiSceneManger.Instance.ActiveSubScene.isLoaded)
                SceneManager.UnloadSceneAsync(MultiSceneManger.Instance.ActiveSubScene);

            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
        }
        
        float displayedProgress = 0f;
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float easedProgress = Mathf.Pow(progress, 0.5f);
            displayedProgress = Mathf.Lerp(displayedProgress, easedProgress, Time.deltaTime * 5f);
            _progressFill.style.width = Length.Percent(displayedProgress * 100f);


            if (operation.progress >= 0.9f && displayedProgress >= 0.98f)
            {
                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
                yield return null;
            }

            yield return null;
        }

        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(Loader.Scene.LoadingScene.ToString()));

        //yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);


        Debug.Log("new scene " + loadedScene.name + " is loaded");
        SceneManager.SetActiveScene(loadedScene);
        
        if (sceneName != Loader.Scene.WorldScene.ToString())
        {
            MultiSceneManger.Instance.ActiveSubScene = SceneManager.GetSceneByName(sceneName);
            Debug.Log("Added subscene: " + MultiSceneManger.Instance.ActiveSubSceneName);

            
        }
        else
        {
            MultiSceneManger.Instance.OnExitToWorldScene(loadedScene);
            PlayerController.LocalInstance.GetComponent<PlayerRoomController>().UpdateRoom(new RoomId { Type = RoomType.None, Id = -1 });
            Debug.Log("Exit to world scene");
        }
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        // this is use to Load from "not-gameplay scene" to "gameplay scene"
        AsyncOperation operation;

        // if loading gameplay scene
        if (sceneName != Loader.Scene.MainMenu.ToString() &&
            sceneName != Loader.Scene.CutScene.ToString() &&
            sceneName != Loader.Scene.CharacterSelectScene.ToString() &&
            sceneName != Loader.Scene.LobbyScene.ToString() &&
            sceneName != Loader.Scene.UIScene.ToString())
        {
            operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.LoadSceneAsync(Loader.Scene.UIScene.ToString(), LoadSceneMode.Additive);
        }
        else // this is for all "not-gameplay scene"
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
        }
        operation.allowSceneActivation = false;
        
        float displayedProgress = 0f;
        
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float easedProgress = Mathf.Pow(progress, 0.5f);
            displayedProgress = Mathf.Lerp(displayedProgress, easedProgress, Time.deltaTime * 5f);
            _progressFill.style.width = Length.Percent(displayedProgress * 100f);

            if (operation.progress >= 0.9f && displayedProgress >= 0.98f)
            {
                
                if (NetworkManager.Singleton.IsHost)
                {
                    if (Loader.TargetScene == Loader.Scene.WorldScene ||
                    Loader.TargetScene == Loader.Scene.CutScene)
                    {
                        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);
                    }
                }

                if (Loader.TargetScene == Loader.Scene.MainMenu)
                {
                    yield return new WaitUntil(() => !NetworkManager.Singleton.IsListening);
                }


                yield return new WaitForSeconds(1f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log("Scene " + sceneName + " loaded successfully");
        if(sceneName != Loader.Scene.MainMenu.ToString() &&
            sceneName != Loader.Scene.CutScene.ToString() &&
            sceneName != Loader.Scene.CharacterSelectScene.ToString() &&
            sceneName != Loader.Scene.LobbyScene.ToString() &&
            sceneName != Loader.Scene.UIScene.ToString())
        {
            if(SceneManager.GetSceneByName(Loader.Scene.LoadingScene.ToString()).isLoaded)
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(Loader.Scene.LoadingScene.ToString())); // if we load in gameplay scene we need to unload this

            //yield return new WaitUntil(() => SceneManager.GetSceneByName(sceneName).isLoaded);
            Scene loadedScene = SceneManager.GetSceneByName(sceneName); // fetch AFTER loading

            Debug.Log("set active scene for: " + loadedScene.name);
            SceneManager.SetActiveScene(loadedScene);
            CinemachineVirtualCamera newFollowCamera = SceneUtils.FindGameObjectByNameInScene("Virtual Camera",loadedScene).GetComponent<CinemachineVirtualCamera>();
            CameraController.Instance.RefreshFollowCamera(newFollowCamera);


            if(sceneName != Loader.Scene.WorldScene.ToString())
            {
                MultiSceneManger.Instance.ActiveSubScene = loadedScene;
                Debug.Log("First scene is not world scene, start to set active subscene: " + MultiSceneManger.Instance.ActiveSubSceneName);
            }
            else
            {
                GameEventsManager.Instance.dataEvents.OnExitToWorldScene(sceneName);
            }

            MultiSceneManger.Instance.FindAllNetworkSingletonAndSpawnOnSceneLoad(loadedScene);
        }
    }
}
