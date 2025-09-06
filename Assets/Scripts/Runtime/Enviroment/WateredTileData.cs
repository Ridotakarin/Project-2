using Unity.Netcode;
using UnityEngine;

[System.Serializable]
[GenerateSerializationForTypeAttribute(typeof(WateredTileData))]
public struct WateredTileData : INetworkSerializable
{
    [SerializeField] public int timeToRemoveTile;
    [SerializeField] public int removeTileCounter;
    [SerializeField] public bool needRemove;

    public WateredTileData(int defaultTime = 1440)
    {
        timeToRemoveTile = defaultTime;
        removeTileCounter = 0;
        needRemove = false;
    }

    public void UpdateTile(int minute)
    {
        removeTileCounter += minute;
        if (removeTileCounter >= timeToRemoveTile)
        {
            needRemove = true;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref timeToRemoveTile);
        serializer.SerializeValue(ref removeTileCounter);
        serializer.SerializeValue(ref needRemove);
    }
}
