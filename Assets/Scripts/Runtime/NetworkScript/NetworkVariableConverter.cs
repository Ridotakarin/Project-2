using Unity.Netcode;
using Unity.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

#region Data Converter
public static class NetworkVariableConverter
{
    
    public static ItemWorldNetworkData ItemWorldToNetwork(ItemWorld itemWorld)
    {
        return new ItemWorldNetworkData
        {
            Id = new FixedString64Bytes(itemWorld.Id),
            ItemName = new FixedString64Bytes(itemWorld.ItemName),
            Quantity = itemWorld.Quantity,
            Position = itemWorld.Position,
            Level = itemWorld.Level
        };
    }

    public static ItemWorld ItemWorldFromNetwork(ItemWorldNetworkData data)
    {
        Item item = ItemDatabase.Instance.GetItemByName(data.ItemName.ToString()); // lookup from string
        return new ItemWorld(
            data.Id.ToString(),
            item,
            data.Quantity,
            data.Position,
            data.Level
        );
    }

    public static List<ItemWorld> ItemWorldListFromNetwork(NetworkList<ItemWorldNetworkData> data)
    {
        List<ItemWorld> itemWorlds = new List<ItemWorld>();
        foreach (var item in data)
        {
            itemWorlds.Add(ItemWorldFromNetwork(item));
        }
        return itemWorlds;
    }
}
#endregion

#region Serializable Definition

    #region Item World
[Serializable]
public struct ItemWorldNetworkData : INetworkSerializable, IEquatable<ItemWorldNetworkData>
{
    public FixedString64Bytes Id;
    public FixedString64Bytes ItemName;
    public int Quantity;
    public Vector3 Position;
    public int Level;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref ItemName);
        serializer.SerializeValue(ref Quantity);
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Level);
    }

    public bool Equals(ItemWorldNetworkData other)
    {
        return Id.Equals(other.Id)
            && ItemName.Equals(other.ItemName)
            && Quantity == other.Quantity
            && Position.Equals(other.Position)
            && Level.Equals(other.Level);
    }

    public override bool Equals(object obj)
    {
        return obj is ItemWorldNetworkData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ItemName, Quantity, Position, Level);
    }
}
#endregion

    #region Vector3Int
[System.Serializable]
[GenerateSerializationForTypeAttribute(typeof(NetworkVector3Int))]
public struct NetworkVector3Int : INetworkSerializable, System.IEquatable<NetworkVector3Int>
{
    public int x;
    public int y;
    public int z;

    public NetworkVector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public NetworkVector3Int(Vector3Int v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }

    public Vector3Int ToVector3Int() => new Vector3Int(x, y, z);

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
        serializer.SerializeValue(ref z);
    }

    public bool Equals(NetworkVector3Int other) => x == other.x && y == other.y && z == other.z;
    public override int GetHashCode() => x ^ (y << 2) ^ (z >> 2);

    
}
#endregion
#endregion
