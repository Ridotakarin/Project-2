using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkTest : MonoBehaviour
{
    [SerializeField] private Button ServerBtn;
    [SerializeField] private Button HostBtn;
    [SerializeField] private Button ClientBtn;
    [SerializeField] private Button DisconnectBtn;

    void Awake()
    {
        ServerBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
            ServerBtn.gameObject.SetActive(false);
            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);
        });
        HostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            ServerBtn.gameObject.SetActive(false);
            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);
        });
        ClientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            ServerBtn.gameObject.SetActive(false);
            HostBtn.gameObject.SetActive(false);
            ClientBtn.gameObject.SetActive(false);
        });
        DisconnectBtn.onClick.AddListener(() =>
        {
            if(NetworkManager.Singleton.IsHost) Debug.Log("Host disconnected when click disconnect.");
            else if(NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost) Debug.Log("Client disconnected.");
            
            NetworkManager.Singleton.Shutdown();
            DisconnectBtn.gameObject.SetActive(true);
        });
    }

}
