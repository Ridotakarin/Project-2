using Unity.Netcode;
using UnityEngine;

public class MainMenuClearUp : MonoBehaviour
{
    private void Awake()
    {
        if (Loader.TargetScene == Loader.Scene.MainMenu)
        {
            if (NetworkManager.Singleton != null) Destroy(NetworkManager.Singleton.gameObject);
            if (GameMultiplayerManager.Instance != null) Destroy(GameMultiplayerManager.Instance.gameObject);
            if (SessionManager.Instance != null) Destroy(SessionManager.Instance.gameObject);
            if (NetworkConnectManager.Instance != null) Destroy(NetworkConnectManager.Instance.gameObject);
        } 
    }
}
