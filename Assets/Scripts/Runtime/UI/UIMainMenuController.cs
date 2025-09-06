using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class UIMainMenuController : MonoBehaviour
{
    //Main menu
    private VisualElement _mainMenuContainer;
    private VisualElement _gameTitleLabel;
    private VisualElement _gameTitleImage;
    private Button _singleplayerButton;
    private Button _multiplayerButton;
    private Button _optionsButton;
    private Button _creditsButton;
    private Button _quitButton;

    private void Awake()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _mainMenuContainer = root.Q<VisualElement>("MainMenuContainer");
        _gameTitleLabel = root.Q<Label>("GameTitleLabel");
        _gameTitleImage = root.Q<VisualElement>("GameTitleImage");
        _singleplayerButton = root.Q<Button>("SingleplayerButton");
        _multiplayerButton = root.Q<Button>("MultiplayerButton");
        _optionsButton = root.Q<Button>("OptionsButton");
        _creditsButton = root.Q<Button>("CreditsButton");
        _quitButton = root.Q<Button>("QuitButton");
    }

    private void Start()
    {
        StartCoroutine(AnimateTitleLoop());
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveMainMenu += OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateMainMenu += OnDisableActive;
        _singleplayerButton.clicked += OnSingleplayerButtonClicked;
        _multiplayerButton.clicked += OnMultiplayerButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;
        _quitButton.clicked += OnQuitGame;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.activeUIPanelEvents.onActiveMainMenu -= OnActive;
        GameEventsManager.Instance.activeUIPanelEvents.onDisActivateMainMenu -= OnDisableActive;
        _singleplayerButton.clicked -= OnSingleplayerButtonClicked;
        _multiplayerButton.clicked -= OnMultiplayerButtonClicked;
        _optionsButton.clicked -= OnOptionsButtonClicked;
        _quitButton.clicked -= OnQuitGame;
    }

    private void OnSingleplayerButtonClicked()
    {
        OnDisableActive();
        GameMultiplayerManager.playMultiplayer = false;
        GameEventsManager.Instance.activeUIPanelEvents.OnActiveSingleplayer();
    }

    public void OnMultiplayerButtonClicked()
    {
        OnDisableActive();
        GameMultiplayerManager.playMultiplayer = true;
        Loader.Load(Loader.Scene.CharacterSelectScene);
    }

    private void OnOptionsButtonClicked()
    {
        OnDisableActive();
        GameEventsManager.Instance.activeUIPanelEvents.OnActiveOptions();
    }

    private void OnActive()
    {
        _mainMenuContainer.RemoveFromClassList("mainmenu-panel-moveleft");
    }

    private void OnDisableActive()
    {
        _mainMenuContainer.AddToClassList("mainmenu-panel-moveleft");
    }

    private void OnQuitGame()
    {
        DataPersistenceManager.Instance.SaveGame();
        Application.Quit();
    }

    private IEnumerator AnimateTitleLoop()
    {
        while (true)
        {
            _gameTitleImage.AddToClassList("game_title_image-style2");
            yield return new WaitForSeconds(1f);
            _gameTitleImage.RemoveFromClassList("game_title_image-style2");
            yield return new WaitForSeconds(1f);
        }
    }
}
