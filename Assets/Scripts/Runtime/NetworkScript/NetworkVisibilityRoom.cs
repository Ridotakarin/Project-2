using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class NetworkVisibilityRoom : NetworkBehaviour
{
    // These could also be NetworkVariables if you want them synced automatically
    public RoomType RoomType;
    // default settings to make all of these object can see each other first
    private RoomId _roomId = new RoomId { Type = RoomType.None, Id = -1 };
    public RoomId RoomId 
    {
        get {  return _roomId; }
        set
        {
            _roomId = value;
            OnRoomChanged();
        }
    }

    public override void OnNetworkSpawn()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != Loader.Scene.MainMenu.ToString() &&
        sceneName != Loader.Scene.CutScene.ToString() &&
        sceneName != Loader.Scene.CharacterSelectScene.ToString() &&
        sceneName != Loader.Scene.LobbyScene.ToString() &&
        sceneName != Loader.Scene.UIScene.ToString())
        {
            if (IsServer)
            {
                // The server handles visibility checks and should subscribe when spawned locally on the server-side.
                NetworkObject.CheckObjectVisibility += CheckVisibility;

                RoomManager.Instance.RegisterVisibilityRoomToList(this);
            }
        }

    }
    public override void OnNetworkDespawn()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != Loader.Scene.MainMenu.ToString() &&
        sceneName != Loader.Scene.CutScene.ToString() &&
        sceneName != Loader.Scene.CharacterSelectScene.ToString() &&
        sceneName != Loader.Scene.LobbyScene.ToString() &&
        sceneName != Loader.Scene.UIScene.ToString())
        {
            if (IsServer)
            {
                NetworkObject.CheckObjectVisibility -= CheckVisibility;
                RoomManager.Instance.UnRegisterVisibilityRoomToList(this);
            }
        }
    }

    public void InitializeRoomInfo(int roomId)
    {
        RoomId = new RoomId { Type = RoomType, Id = roomId };
    }

    // Called by server to check if the player can see this object
    private bool CheckVisibility(ulong clientId)
    {
        if (!IsSpawned)
        {
            return false;
        }
        RoomId clientRoom = RoomManager.Instance.GetRoomIdOfClient(clientId);
        RoomId objectRoom = RoomId;

        // Only visible if room IDs match
        return clientRoom == objectRoom;
    }

    // call this when update RoomId
    public void OnRoomChanged()
    {
        foreach (var clientId in NetworkManager.ConnectedClientsIds)
        {
            if (OwnerClientId == clientId && NetworkObject.IsOwner)
                continue;

            var shouldBeVisibile = CheckVisibility(clientId);
            var isVisibile = NetworkObject.IsNetworkVisibleTo(clientId);
            if (shouldBeVisibile && !isVisibile)
            {
                // Note: This will invoke the CheckVisibility check again
                NetworkObject.NetworkShow(clientId);
            }
            else if (!shouldBeVisibile && isVisibile)
            {
                NetworkObject.NetworkHide(clientId);
            }
        }
    }


}
