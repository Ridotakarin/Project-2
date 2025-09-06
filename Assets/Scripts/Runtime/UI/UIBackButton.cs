using System.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIBackButton : MonoBehaviour
{
    public Button leaveButton;
    public Button leaveNetworkButton;

    private void OnEnable()
    {
        GameEventsManager.Instance.networkEvents.onSessionCreate += ShowButton;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.networkEvents.onSessionCreate -= ShowButton;
    }

    private void Start()
    {
        ShowButton();

        if (leaveButton != null) leaveButton.onClick.AddListener(OnBackButtonClicked);
        if (leaveButton != null) leaveNetworkButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void ShowButton()
    {
        if (leaveButton == null || leaveNetworkButton == null) return;
        if (SessionManager.Instance.ActiveSession == null)
        {
            leaveButton.gameObject.SetActive(true);
            leaveNetworkButton.gameObject.SetActive(false);
        }
        else
        {
            leaveButton.gameObject.SetActive(false);
            leaveNetworkButton.gameObject.SetActive(true);
        }
    }

    private void OnBackButtonClicked()
    {
        SessionManager.Instance.DisInitAndSignOut();
        if (NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
        Loader.Load(Loader.Scene.MainMenu);

        GameEventsManager.Instance.networkEvents.OnSessionCreate();
    }
}
