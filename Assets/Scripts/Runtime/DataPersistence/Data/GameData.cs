using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [SerializeField] private string _saveFileName;
    [SerializeField] private string _lastUpdate;
    [SerializeField] private PlayerData _playerData;
    [SerializeField] private Inventory _inventoryData;
    [SerializeField] private EnvironmentalStatus _eStatus;
    [SerializeField] private ListItemWorld _listItemWold;
    [SerializeField] private TileSaveData _tileSaveData;
    [SerializeField] private CropsSaveData _cropsSaveData;
    [SerializeField] private GameFlowData _gameFlowData;
    [SerializeField] private FarmAnimalSaveDataCollection _farmAnimalSaveDataCollection;
    [SerializeField] private PlacedTileData _placedTileData;
    public string SaveFileName
    { get { return _saveFileName; } }
    public DateTime LastUpdate
    { get { return Convert.ToDateTime(_lastUpdate); } }
    public PlayerData PlayerData
    { get { return _playerData; } }

    public Inventory InventoryData
    { get { return _inventoryData; } }

    public EnvironmentalStatus EnviromentData
    { get { return _eStatus; } }

    public ListItemWorld ListItemWold
    { get { return _listItemWold; } }

    public TileSaveData TileSaveData
    { get { return _tileSaveData; } }

    public CropsSaveData CropsSaveData
    { get { return _cropsSaveData; } }

    public GameFlowData GameFlowData
    { get { return _gameFlowData; } }

    public FarmAnimalSaveDataCollection FarmAnimalSaveDataCollection
    { get { return _farmAnimalSaveDataCollection; } }

    public PlacedTileData PlacedTileData
    { get { return _placedTileData; } }
    public GameData()
    {
        this._saveFileName = string.Empty;
        this._lastUpdate = DateTime.Now.ToString("O");
        this._playerData = new PlayerData();
        this._inventoryData = new Inventory();
        this._eStatus = new EnvironmentalStatus();
        this._listItemWold = new ListItemWorld();
        this._tileSaveData = new TileSaveData();
        this._cropsSaveData = new CropsSaveData();
        this._gameFlowData = new GameFlowData();
        this._farmAnimalSaveDataCollection = new FarmAnimalSaveDataCollection();
        this._placedTileData = new PlacedTileData();
    }

    public void SetPlayerData(PlayerData playerData)
    {
        this._playerData = playerData;
    }

    public void SetInventoryData(Inventory inventoryData)
    {
        this._inventoryData = inventoryData;
    }

    public void SetSeason(EnvironmentalStatus status)
    { 
        this._eStatus = status; 
    }

    public void SetListItemWorld(ListItemWorld itemWorld)
    {
        this._listItemWold = itemWorld;
    }

    public void SetTiles(TileSaveData tileSaveData)
    {
        this._tileSaveData = tileSaveData;
    }

    public void SetCropsData(CropsSaveData cropsSaveData)
    {
        this._cropsSaveData = cropsSaveData;
    }

    public void SetSaveFileName(string saveFileName)
    {
        this._saveFileName = saveFileName;
    }

    public void SetLastUpdate(DateTime lastUpdate)
    {
        this._lastUpdate = lastUpdate.ToString("O");
    }

    public void SetGameFlowData(GameFlowData gameFlowData)
    {
        this._gameFlowData = gameFlowData;
    }

    public void SetFarmAnimalSaveDataCollection(FarmAnimalSaveDataCollection farmAnimalSaveDataCollection)
    {
        this._farmAnimalSaveDataCollection = farmAnimalSaveDataCollection;
    }

    public void SetPlacedTileData(PlacedTileData placedTileData)
    {
        this._placedTileData = placedTileData;
    }
}
