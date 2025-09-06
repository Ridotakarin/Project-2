using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDatabase : PersistentSingleton<TileDatabase>
{
    [SerializeField]
    private List<TileBase> _tileBases;

    [SerializeField]
    private List<Tilemap> _tilemaps;

    public TileBase GetTileByName(string tileName)
    {
        foreach(TileBase tile in _tileBases)
        {
            if (tile.name == tileName)
            {
                return tile;
            }
        }
        Debug.LogWarning($"Tile with name {tileName} not found in the database.");
        return null;
    }

    public void SetListTilemap(List<Tilemap> tilemaps)
    {
        _tilemaps = tilemaps;
    }

    public Tilemap GetTilemapByName(string tilemapName)
    {
        foreach(Tilemap tilemap in _tilemaps)
        {
            if (tilemap.name == tilemapName)
            {
                return tilemap;
            }
        }
        Debug.LogWarning($"Tilemap with name {tilemapName} not found in the database.");
        return null;
    }
}
