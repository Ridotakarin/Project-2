using Unity.Netcode;
using UnityEngine;

public class MyNetworkManager : MonoBehaviour
{
    private void Awake()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

    }
}
