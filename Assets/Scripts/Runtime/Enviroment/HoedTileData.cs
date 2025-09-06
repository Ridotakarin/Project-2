using System.Diagnostics;
using Unity.Netcode;

[System.Serializable]
[GenerateSerializationForTypeAttribute(typeof(HoedTileData))]
public struct HoedTileData : INetworkSerializable
{
    public int timeToRemoveTile;
    public int removeTileCounter;
    public bool hasSomethingOn;
    public bool needRemove;

    public HoedTileData(int defaultTime = 4320)
    {
        timeToRemoveTile = defaultTime;
        removeTileCounter = 0;
        hasSomethingOn = false;
        needRemove = false;
    }

    public void UpdateTile(int minute)
    {
        if (!hasSomethingOn)
        {
            removeTileCounter += minute;
            if (removeTileCounter >= timeToRemoveTile)
            {
                needRemove = true;
            }
        }
        else
        {
            removeTileCounter = 0;
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref timeToRemoveTile);
        serializer.SerializeValue(ref removeTileCounter);
        serializer.SerializeValue(ref hasSomethingOn);
        serializer.SerializeValue(ref needRemove);
    }
}
