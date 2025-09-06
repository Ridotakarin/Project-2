using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DataPersistenceManager : PersistentSingleton<DataPersistenceManager>
{
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileId = false;
    [SerializeField] private string testSelectedProfileId = "New World";

    [Header("File Storage Config")]
    [SerializeField] private string fileName = "data";
    [SerializeField] private bool useEncryption;

    [Header("Auto Saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    private GameData gameData;
    public GameData GameData
    {
        get { return gameData; }
    }
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private string selectedProfileId = "";

    private Coroutine autoSaveCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        InitializeSelectedProfileId(null);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameEventsManager.Instance.dataEvents.onInitialized += InitializeSelectedProfileId;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (scene.name == Loader.Scene.WorldScene.ToString() && 
        //    SceneManager.GetSceneByName("WorldScene").isLoaded) return;
        GameEventsManager.Instance.inputReader.EnableControl();
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void ChangeSelectedProfileId(string newProfileId)
    {
        // update the profile to use for saving and loading
        this.selectedProfileId = newProfileId;
        
        //LoadGame();
    }

    public void DeleteProfileData(string profileId)
    {
        dataHandler.Delete(profileId);
        
        InitializeSelectedProfileId(null);
        
        LoadGame();
    }

    private void InitializeSelectedProfileId(string profileId)
    {
        if (overrideSelectedProfileId || profileId == "")
        {
            this.selectedProfileId = testSelectedProfileId;
            return;
        }

        this.selectedProfileId = profileId;
        Debug.Log($"Selected Profile ID: {selectedProfileId}");
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load(selectedProfileId);

        // start a new game if the data is null and we're configured to initialize data for debugging purposes
        if (this.gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        // if no data can be loaded, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
            return;
        }

        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        if (!NetworkManager.Singleton.IsServer)
        {
            SyncWorldDataToPlayerServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void SyncWorldDataToPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;

        var rpcParamsForClient = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        };
        SyncWorldDataToPlayerClientRpc(rpcParamsForClient);
    }

    [ClientRpc]
    private void SyncWorldDataToPlayerClientRpc(ClientRpcParams clientRpcParams = default)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.WorldScene.ToString()) return;

        TileManager.Instance.SyncTileOnLateJoin();
        ItemWorldManager.Instance.SyncItemWorldOnLateJoin();
        CropManager.Instance.SyncCropsOnLateJoin();

    }

    public void SaveGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == Loader.Scene.MainMenu.ToString() ||
            currentSceneName == Loader.Scene.LoadingScene.ToString() ||
            currentSceneName == Loader.Scene.LobbyScene.ToString() ||
            currentSceneName == Loader.Scene.CharacterSelectScene.ToString() ||
            disableDataPersistence) return;
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        gameData.SetLastUpdate(System.DateTime.Now);

        dataHandler.Save(gameData, selectedProfileId);
        Debug.Log("Saved Game Data");
    }

    private void OnApplicationQuit()
    {
        SettingsManager.Instance.SaveData();
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    public void CaptureScreenshot()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        RenderTexture renderTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, -10);

        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        RenderTexture.active = renderTexture;
        Texture2D screenshot = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);
        screenshot.Apply();

        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        string screenFileName = "_screenshot.png";

        dataHandler.SaveScreenshot(selectedProfileId, screenshot, screenFileName);
    }

    public Texture2D LoadScreenshot(string profileId)
    {
        return dataHandler.GetScreenshot(profileId);
    }
}
