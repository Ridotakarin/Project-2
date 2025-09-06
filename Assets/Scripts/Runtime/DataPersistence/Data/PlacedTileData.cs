using UnityEngine;

[System.Serializable]
public class PlacedTileData
{
    [SerializeField]
    public SerializableDictionary<Vector3Int, string> _placedTile;

    public PlacedTileData()
    {
        _placedTile = new SerializableDictionary<Vector3Int, string>();
    }

    public PlacedTileData(SerializableDictionary<Vector3Int, string> placedTile)
    {
        _placedTile = placedTile;
    }
}
