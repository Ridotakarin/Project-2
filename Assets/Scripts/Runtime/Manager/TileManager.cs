using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static CaveGenerator;

public class TileManager : NetworkPersistentSingleton<TileManager>, IDataPersistence
{
    [SerializeField] private List<Tilemap> tilemaps = new List<Tilemap>();
    [SerializeField] private List<RuleTile> ruleTiles = new List<RuleTile>();

    private string FarmGroundTilemapName = "FarmGround";
    private string WateredGroundTilemapName = "WateredGround";
    private string HoedTileName = "Hoed Tile";
    private string WateredTileName = "Watered Tile";

    private SerializableDictionary<Vector3Int, HoedTileData> _hoedTiles = new SerializableDictionary<Vector3Int, HoedTileData>();
    public SerializableDictionary<Vector3Int, HoedTileData> HoedTiles
    {
        get { return _hoedTiles; }
        set { _hoedTiles = value; }
    }
   

    private SerializableDictionary<Vector3Int, WateredTileData> _wateredTiles = new SerializableDictionary<Vector3Int, WateredTileData>();
    public SerializableDictionary<Vector3Int, WateredTileData> WateredTiles
    {
        get { return _wateredTiles; }
        set { _wateredTiles = value; }
    }

    public NetworkDictionary<NetworkVector3Int, HoedTileData> HoedTilesNetwork;
    public NetworkDictionary<NetworkVector3Int, WateredTileData> WateredTilesNetwork;


    private TileSaveData _tileSaveData = new TileSaveData();

    private void OnEnable()
    {
        if(HoedTilesNetwork == null)
        HoedTilesNetwork = new NetworkDictionary<NetworkVector3Int, HoedTileData>();
        if(WateredTilesNetwork == null)
        WateredTilesNetwork = new NetworkDictionary<NetworkVector3Int, WateredTileData>();
    }

    public void SyncTileOnLateJoin()
    {
        foreach (var hoedTile in HoedTilesNetwork)
        {
            var tilePos = hoedTile.Key.ToVector3Int();
            var tileMap = GetTilemapByName(FarmGroundTilemapName);
            var tile = GetRuleTileByName(HoedTileName);

            tileMap.SetTile(tilePos, tile);
        }

        foreach(var wateredTile in WateredTilesNetwork)
        {
            var tilePos = wateredTile.Key.ToVector3Int();
            var tileMap = GetTilemapByName(WateredGroundTilemapName);
            var tile = GetRuleTileByName(WateredTileName);

            tileMap.SetTile(tilePos, tile);
        }

    }

    private Tilemap GetTilemapByName(string tilemapName)
    {
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.name == tilemapName)
            {
                return tilemap;
            }
        }
        return null;
    }
    private RuleTile GetRuleTileByName(string ruleTileName)
    {
        foreach(var ruleTile in ruleTiles)
        {
            if (ruleTile.name == ruleTileName)
            {
                return ruleTile;
            }
        }
        return null;
    } 
    public void UpdateAllTileStatus(int minute)
    {
        //var sceneName = SceneManager.GetActiveScene().name;
        //if (sceneName != "WorldScene" || sceneName != "GameplayScene") return;

        List<NetworkVector3Int> hoedTilesToRemove = new();
        List<NetworkVector3Int> wateredTilesToRemove = new();

        // First pass: update hoed tiles
        foreach (var hoedTile in HoedTilesNetwork)
        {
            NetworkVector3Int hoedPosition = hoedTile.Key;
            HoedTileData hoedTileData = hoedTile.Value;

            if (WateredTilesNetwork.ContainsKey(hoedPosition) || CropManager.Instance.PlantedCropsNetwork.ContainsKey(hoedPosition))
                hoedTileData.hasSomethingOn = true;
            else if(!WateredTilesNetwork.ContainsKey(hoedPosition) && !CropManager.Instance.PlantedCropsNetwork.ContainsKey(hoedPosition))
                hoedTileData.hasSomethingOn = false;
            hoedTileData.UpdateTile(minute);
            HoedTilesNetwork[hoedPosition] = hoedTileData;

            if (hoedTileData.needRemove)
            {
                hoedTilesToRemove.Add(hoedPosition);
            }
        }

        // Remove hoed tiles **after** the loop
        foreach (var pos in hoedTilesToRemove)
        {
            ModifyTile(pos.ToVector3Int(), FarmGroundTilemapName);
        }

        // First pass: update watered tiles
        foreach (var wateredTile in WateredTilesNetwork)
        {
            NetworkVector3Int wateredPosition = wateredTile.Key;
            WateredTileData wateredTileData = wateredTile.Value;

            wateredTileData.UpdateTile(minute);
            WateredTilesNetwork[wateredPosition] = wateredTileData;


            if (wateredTileData.needRemove)
            {
                wateredTilesToRemove.Add(wateredPosition);
            }
        }

        // Remove watered tiles **after** the loop
        foreach (var pos in wateredTilesToRemove)
        {
            ModifyTile(pos.ToVector3Int(), WateredGroundTilemapName);
        }
    }


    public void ModifyTile(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
        //var sceneName = SceneManager.GetActiveScene().name;
        //if (sceneName != Loader.Scene.WorldScene.ToString()) return;
        RequestToModifyTileServerRpc(tilePos, tilemapName, ruleTileName);

        //HoedTileData newHoedTile = new HoedTileData();
        //_hoedTiles.Add(tilePos, newHoedTile);

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToModifyTileServerRpc(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
        var targetTilemap = GetTilemapByName(tilemapName);
        var ruleTile = GetRuleTileByName(ruleTileName);

        if (targetTilemap == null) return;

        NetworkVector3Int networkTilePos = new NetworkVector3Int(tilePos);
        if (ruleTile != null)
        {
            switch (tilemapName)
            {
                case "FarmGround":
                    if (!HoedTilesNetwork.ContainsKey(networkTilePos))
                    {
                        HoedTilesNetwork.Add(networkTilePos, new HoedTileData(1000));
                    }
                    break;

                case "WateredGround":
                    if (!WateredTilesNetwork.ContainsKey(networkTilePos))
                    {
                        WateredTilesNetwork.Add(networkTilePos, new WateredTileData(2000));
                    }
                    break;
            }
        }
        else
        {
            // ruleTile is null => remove the tile
            switch (tilemapName)
            {
                case "FarmGround":
                    if (HoedTilesNetwork.ContainsKey(networkTilePos))
                    {
                        HoedTilesNetwork.Remove(networkTilePos);
                    }
                    if (WateredTilesNetwork.ContainsKey(networkTilePos))
                    {
                        WateredTilesNetwork.Remove(networkTilePos);
                    }
                    break;

                case "WateredGround":
                    if (WateredTilesNetwork.ContainsKey(networkTilePos))
                    {
                        WateredTilesNetwork.Remove(networkTilePos);
                    }
                    break;
            }
        }

        // Then sync visuals for clients (clients don't touch network vars)
        ApplyTileForPlayersClientRpc(tilePos, tilemapName, ruleTileName);
    }

    [ClientRpc]
    private void ApplyTileForPlayersClientRpc(Vector3Int tilePos, string tilemapName, string ruleTileName = null)
    {
        var targetTilemap = GetTilemapByName(tilemapName);
        var ruleTile = GetRuleTileByName(ruleTileName);

        if (targetTilemap != null)
        {
            targetTilemap.SetTile(tilePos, ruleTile);
        }
    }

    private IEnumerator SetAllTileDataToNetworkData()
    {

        yield return new WaitUntil(() => IsSpawned);
        foreach (var hoedTile in HoedTiles)
        {
            HoedTilesNetwork.Add(new NetworkVector3Int(hoedTile.Key), hoedTile.Value); // add to network data
        }

        foreach (var wateredTile in WateredTiles)
        {
            WateredTilesNetwork.Add(new NetworkVector3Int(wateredTile.Key), wateredTile.Value); // add to network data
        }
    }

    private void SetAllTileNetworkDataToLocal()
    {
        HoedTiles.Clear(); // clear local data
        WateredTiles.Clear(); // clear local data
        foreach (var hoedTile in HoedTilesNetwork)
        {
            HoedTiles.Add(hoedTile.Key.ToVector3Int(), hoedTile.Value); // add to local data
        }

        foreach (var wateredTile in WateredTilesNetwork)
        {
            WateredTiles.Add(wateredTile.Key.ToVector3Int(), wateredTile.Value); // add to local data
        }
    }

    public void LoadData(GameData data)
    {
        if (!IsHost) return;

        //if (SceneManager.GetSceneByName("WorldScene").isLoaded) return;
        //Debug.Log("Co load");
        //Tilemap listtm = GameObject.Find("FarmGround").GetComponent<Tilemap>();
        //tilemaps.Add(listtm);
        //var listnj = GameObject.Find("WateredGround").GetComponent<Tilemap>();
        //tilemaps.Add(listnj);
        HoedTiles = data.TileSaveData.HoedTiles;
        WateredTiles = data.TileSaveData.WateredTiles;
        StartCoroutine(SetAllTileDataToNetworkData());
        StartCoroutine(ApplyTileUpdatesOnLoadGame());
        GameEventsManager.Instance.enviromentStatusEvents.onTimeIncrease += UpdateAllTileStatus;
    }
    private IEnumerator ApplyTileUpdatesOnLoadGame()
    {
        yield return new WaitForEndOfFrame(); // Wait until rendering is done

        foreach (var hoedTile in HoedTilesNetwork)
        {
            ModifyTile(hoedTile.Key.ToVector3Int(), FarmGroundTilemapName, HoedTileName);
        }

        foreach (var wateredTile in WateredTilesNetwork)
        {
            ModifyTile(wateredTile.Key.ToVector3Int(), WateredGroundTilemapName, WateredTileName);
        }

    }
    public void SaveData(ref GameData data)
    {
        if (!IsHost) return;
        SetAllTileNetworkDataToLocal();
        _tileSaveData.SetTilesData(HoedTiles, WateredTiles);
        data.SetTiles(_tileSaveData);

        GameEventsManager.Instance.enviromentStatusEvents.onTimeIncrease -= UpdateAllTileStatus;
    }

}
