using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkConnectManager : NetworkPersistentSingleton<NetworkConnectManager>
{
    private NetworkList<PlayerDataNetwork> connectedPlayers = new NetworkList<PlayerDataNetwork>();

    public override void OnNetworkSpawn()
    {
        StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        if (NetworkManager.Singleton == null)
            yield return new WaitUntil(() => NetworkManager.Singleton != null);

        yield return new WaitUntil(() => NetworkManager.Singleton.IsListening);

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

        Debug.Log("NetworkConnectManager initialized and callbacks subscribed.");
    }

    // Runs only on the server
    [ServerRpc(RequireOwnership = false)]
    public void OnClientConnectedServerRpc(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            Debug.LogError($"Client {clientId} not found in ConnectedClients.");
            return;
        }

        var networkObject = client.PlayerObject;
        var playerController = networkObject.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("PlayerController is null");
            return;
        }

        bool isHost = clientId == NetworkManager.ServerClientId;

        //if (isHost)
        //{
        //    connectedPlayers = new NetworkList<PlayerDataNetwork>();
        //    NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        //    NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        //}
        if (connectedPlayers.Contains(playerController.playerDataNetwork))
        {
            Debug.Log($"PlayerController already exists for clientId {clientId} or max players reached. Skipping connection.");
            return;
        }

        connectedPlayers.Add(playerController.playerDataNetwork);
        Debug.Log($"Client {clientId} connected. PlayerController: {playerController.playerDataNetwork.playerName}, Character ID: {playerController.playerDataNetwork.characterId}");
        SetPlayerNameServerRpc(playerController.playerDataSO.playerName.ToString());
        SetCharactersAnimatorServerRpc(playerController.playerDataSO.characterId);
    }

    public void OnClientConnected(ulong clientId)
    {
        if (IsHost) LoadSceneClientRpc();
    }

    [ClientRpc]
    public void LoadSceneClientRpc()
    {
        if (!IsHost) Loader.Load(Loader.Scene.WorldScene); 
    }

    void OnClientDisconnected(ulong clientId)
    {   
        Debug.Log($"Client {clientId} disconnected.");
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (connectedPlayers[(int)clientId].clientId == clientId)
        {
            connectedPlayers.RemoveAt((int)clientId);
        }
        else
        {
            Debug.LogWarning($"PlayerController not found for clientId {clientId}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerDataNetwork playerData = connectedPlayers[playerDataIndex];

        playerData.playerName = playerName;

        connectedPlayers[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetCharactersAnimatorServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log($"charID: {characterId}");
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerDataNetwork playerData = connectedPlayers[playerDataIndex];

        playerData.characterId = characterId;

        connectedPlayers[playerDataIndex] = playerData;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        return (int)clientId;
    }
}
