using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostShutdown : MonoBehaviour
{
    public async void OnShutDownHost()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
        {
            Debug.LogWarning("NetworkManager is not initialized or not listening.");
            return;
        }

        await SessionManager.Instance.InitializeAndSignIn();
        NetworkManager.Singleton.Shutdown();

        GameEventsManager.Instance.networkEvents.OnSessionCreate();
    }
}
