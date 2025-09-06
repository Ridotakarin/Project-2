using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Tilemap/MapLayerData")]
public class MapLayerData : ScriptableObject
{
    public string layerName;
    public TileInfo[] tiles;
}

[System.Serializable]
public class TileInfo
{
    public Vector3Int position;
    public TileBase tile;
}