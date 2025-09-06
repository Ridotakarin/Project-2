using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UISaveFilePanelController : MonoBehaviour
{
    private VisualElement _saveFilePanel;
    private ScrollView _saveFileList;
    private Button _newGameButton;
    private Button _loadGameButton;
    private Button _saveFileBackButton;

    private VisualElement _initFileNamePanel;
    private TextField _fileNameInput;
    private Button _confirmButton;
    private Button _cancelButton;

    public VisualTreeAsset _saveFileData;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _saveFilePanel = root.Q<VisualElement>("SaveFilePanel");
        _saveFileList = root.Q<ScrollView>("SaveFileList");
        _newGameButton = root.Q<Button>("NewGameButton");
        _loadGameButton = root.Q<Button>("LoadGameButton");
        _saveFileBackButton = root.Q<Button>("SaveFileBackButton");

        _initFileNamePanel = root.Q<VisualElement>("InitFileNamePanel");
        _fileNameInput = _initFileNamePanel.Q<TextField>("FileNameInput");
        _confirmButton = root.Q<Button>("ConfirmButton");
        _cancelButton = root.Q<Button>("CancelButton");
    }

    private void Start()
    {
        _loadGameButton.SetEnabled(false);
        PopulateSaveFiles();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveSingleplayer += OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateSingleplayer += OnDisableActive;
        _newGameButton.clicked += OnNewGameButtonClicked;
        _loadGameButton.clicked += OnLoadGameButtonClicked;
        _saveFileBackButton.clicked += OnBackButtonClicked;

        _confirmButton.clicked += OnConfirmButtonClicked;
        _cancelButton.clicked += OnCancelButtonClicked;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveSingleplayer -= OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateSingleplayer -= OnDisableActive;
        _newGameButton.clicked -= OnNewGameButtonClicked;
        _loadGameButton.clicked -= OnLoadGameButtonClicked;
        _saveFileBackButton.clicked -= OnBackButtonClicked;

        _confirmButton.clicked -= OnConfirmButtonClicked;
        _cancelButton.clicked -= OnCancelButtonClicked;
    }

    private void OnNewGameButtonClicked()
    {
        _initFileNamePanel.style.display = DisplayStyle.Flex;
    }

    private void OnLoadGameButtonClicked()
    {
        _initFileNamePanel.style.display = DisplayStyle.None;
        _saveFilePanel.style.display = DisplayStyle.None;

        GameMultiplayerManager.playMultiplayer = false;
        Loader.Load(Loader.Scene.WorldScene);
        NetworkManager.Singleton.StartHost();
        DataPersistenceManager.Instance.LoadGame();
    }

    private void OnBackButtonClicked()
    {
        _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
        GameEventsManager.Instance.activeUIPanelEvents.OnActiveMainMenu();
    }

    private void OnConfirmButtonClicked()
    {
        _initFileNamePanel.style.display = DisplayStyle.None;
        _saveFilePanel.style.display = DisplayStyle.None;
        GameEventsManager.Instance.dataEvents.OnInitialized(_fileNameInput.value.ToString());
        
        GameMultiplayerManager.playMultiplayer = false;
        Loader.Load(Loader.Scene.CharacterSelectScene);
    }

    private void OnCancelButtonClicked()
    {
        _fileNameInput.value = string.Empty;
        _initFileNamePanel.style.display = DisplayStyle.None;
    }

    private void OnSaveFileSelected(IEnumerable<object> selectedItems, string profileId)
    {
        _loadGameButton.SetEnabled(selectedItems != null && selectedItems.Any());
        GameEventsManager.Instance.dataEvents.OnInitialized(profileId);
        DataPersistenceManager.Instance.LoadGame();
    }

    private void PopulateSaveFiles()
    {
        Dictionary<string, GameData> saveFiles = DataPersistenceManager.Instance.GetAllProfilesGameData();
        var sortedByValue = saveFiles.OrderBy(pair => pair.Value.LastUpdate);
        _saveFileList.Clear();
        foreach (var saveFile in sortedByValue)
        {
            var saveFileData = _saveFileData.CloneTree();
            saveFileData.Q<VisualElement>("FileImage").style.backgroundImage = DataPersistenceManager.Instance.LoadScreenshot(saveFile.Key);
            saveFileData.Q<Label>("FileName").text = saveFile.Key;
            saveFileData.Q<Label>("FileDate").text = saveFile.Value.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss");
            saveFileData.Q<Button>("FileDataButton").clicked += () => OnSaveFileSelected(new List<object> { saveFiles }, saveFile.Key);
            _saveFileList.Add(saveFileData);
        }
    }    

    private void OnActive()
    {
        _saveFilePanel.AddToClassList("savefile-panel-moveleft");
    }

    private void OnDisableActive()
    {
        _saveFilePanel.RemoveFromClassList("savefile-panel-moveleft");
    }
}
