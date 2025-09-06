using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoomManager : NetworkPersistentSingleton<RoomManager>
{
    private Dictionary<ulong, RoomId> clientRoomMap = new();

    private List<NetworkVisibilityRoom> _networkVisibilityObjects = new();

    // these 2 method will only run by server so only server obtains these value
    public void SetClientRoom(ulong clientId, RoomId roomId)
    {
        clientRoomMap[clientId] = roomId;
    }

    public RoomId GetRoomIdOfClient(ulong clientId)
    {
        if (clientRoomMap.TryGetValue(clientId, out var room))
            return room;
        return new RoomId { Type = RoomType.None, Id = 0 };
    }

    public void RegisterVisibilityRoomToList(NetworkVisibilityRoom obj)
    {
        if (!_networkVisibilityObjects.Contains(obj))
            _networkVisibilityObjects.Add(obj);
    }

    public void UnRegisterVisibilityRoomToList(NetworkVisibilityRoom obj)
    {
        if (_networkVisibilityObjects.Contains(obj))
            _networkVisibilityObjects.Remove(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RefreshCheckOnAllVisibilityObjectsServerRpc()
    {
        foreach(var obj in _networkVisibilityObjects)
        {
            obj.OnRoomChanged();
        }
    }
}

#region define RoomId
public enum RoomType
{
    None,
    House,
    Cave
}

public struct RoomId
{
    public RoomType Type;
    public int Id;

    public override bool Equals(object obj)
    {
        if (obj is RoomId other)
            return Type == other.Type && Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode() ^ Id.GetHashCode();
    }

    public static bool operator ==(RoomId lhs, RoomId rhs) => lhs.Equals(rhs);
    public static bool operator !=(RoomId lhs, RoomId rhs) => !lhs.Equals(rhs);
}
#endregion
