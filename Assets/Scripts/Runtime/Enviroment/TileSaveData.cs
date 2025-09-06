using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileSaveData
{
    [SerializeField] private SerializableDictionary<Vector3Int, HoedTileData> _hoedTiles;
    [SerializeField] private SerializableDictionary<Vector3Int, WateredTileData> _wateredTiles;

    public SerializableDictionary<Vector3Int, HoedTileData> HoedTiles
    {
        get { return _hoedTiles; }
        set { _hoedTiles = value; }
    }

    public SerializableDictionary<Vector3Int, WateredTileData> WateredTiles
    {
        get { return _wateredTiles; }
        set { _wateredTiles = value; }
    }


    public TileSaveData()
    {
        _hoedTiles = new SerializableDictionary<Vector3Int, HoedTileData>();
        _wateredTiles = new SerializableDictionary<Vector3Int, WateredTileData>();
    }

    public void SetTilesData(SerializableDictionary<Vector3Int, HoedTileData> hoedTiles,
        SerializableDictionary<Vector3Int, WateredTileData> wateredTiles)
    {
        HoedTiles = hoedTiles; 
        WateredTiles = wateredTiles;
    }

}
