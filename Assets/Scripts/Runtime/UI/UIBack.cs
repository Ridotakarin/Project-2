using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIBack : MonoBehaviour
{
    public Button leaveButton;

    private void Start()
    {
        leaveButton.onClick.AddListener(OnBackButtonClicked);
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
