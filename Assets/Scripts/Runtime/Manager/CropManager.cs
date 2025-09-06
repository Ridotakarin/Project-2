
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEngine.Rendering.Universal.PixelPerfectCamera;

public class CropManager : NetworkPersistentSingleton<CropManager>, IDataPersistence
{
    public Tilemap cropTilemap;

    private SerializableDictionary<Vector3Int, CropData> _plantedCrops = new SerializableDictionary<Vector3Int, CropData>();
    public SerializableDictionary<Vector3Int, CropData> PlantedCrops
    {
        get { return _plantedCrops; }
        set { _plantedCrops = value; }
    }

    public NetworkDictionary<NetworkVector3Int, CropData> PlantedCropsNetwork;
    private CropsSaveData _cropsSaveData = new CropsSaveData();

    private void OnEnable()
    {
        if (PlantedCropsNetwork == null)
            PlantedCropsNetwork = new NetworkDictionary<NetworkVector3Int, CropData>();
    }

    public void SyncCropsOnLateJoin()
    {
        foreach (var crop in PlantedCropsNetwork)
        {
            var cropPos = crop.Key.ToVector3Int();
            var cropName = crop.Value.CropSeedName.ToString();
            var stage = crop.Value.CurrentStage;
            var cropSeed = ItemDatabase.Instance.GetItemByName(cropName);

            cropTilemap.SetTile(cropPos, cropSeed.CropSetting.growthStages[stage]);
        }
    }
    /// <summary>
    /// this method will update date on the list and the tilemap too
    /// </summary>
    /// <param name="plantPosition"></param>
    /// <param name="cropName"></param>
    /// <param name="stage"></param>
    public void TryModifyCrop(Vector3Int plantPosition, string cropName, int stage)
    {
        RequestToModifyCropServerRpc(plantPosition, cropName, stage);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToModifyCropServerRpc(Vector3Int plantPosition, string cropName, int stage)
    {
        var networkPos = new NetworkVector3Int(plantPosition);
        if (cropName == null && PlantedCropsNetwork.ContainsKey(networkPos)) // xoa
            PlantedCropsNetwork.Remove(networkPos);
        else
        {
            Item cropSeed = ItemDatabase.Instance.GetItemByName(cropName);
            if (stage == 1) // plant
            {
                if (!PlantedCropsNetwork.ContainsKey(networkPos))
                {
                    var stageCount = cropSeed.CropSetting.growthStages.Length;
                    var timeToChangeStage = cropSeed.CropSetting.TimeToGrowth / stageCount - 1;
                    var season = cropSeed.CropSetting.season;
                    var cropProductName = cropSeed.CropSetting.cropProductName;
                    var canReHarvest = cropSeed.CropSetting.canReHarvest;

                    CropData newCrop = new CropData(stageCount, timeToChangeStage, season, cropSeed.itemName, cropProductName, canReHarvest);

                    PlantedCropsNetwork.Add(networkPos, newCrop);
                }
            }
            else
            {
                var cropCurrentData = PlantedCropsNetwork[networkPos];
                if (stage != cropCurrentData.CurrentStage)
                {
                    cropCurrentData.CurrentStage = stage; // update stage
                    PlantedCropsNetwork[networkPos] = cropCurrentData;
                }
            }
        }

        ModifyCropClientRpc(plantPosition, cropName, stage);
    }

    [ClientRpc]
    private void ModifyCropClientRpc(Vector3Int plantPosition, string cropName, int stage)
    {
        if(cropName == null) ChangeCropStage(plantPosition, null, 0); //name null = delete
        else
        {
            Item cropSeed = ItemDatabase.Instance.GetItemByName(cropName);
            if (stage == 1)
                PlantCrop(plantPosition, cropSeed);
            else ChangeCropStage(plantPosition, cropSeed, stage);
        }
        
    }

    public void PlantCrop(Vector3Int plantPosition, Item cropSeed)
    {
        AudioManager.Instance.PlaySFX("walk_grass_2");
        cropTilemap.SetTile(plantPosition, cropSeed.CropSetting.growthStages[1]); // seed stage
    }

    private void ChangeCropStage(Vector3Int plantPosition, Item crop, int stage)
    {
        if(crop == null)
        {
            cropTilemap.SetTile(plantPosition, null);
        }
        else
        {
            cropTilemap.SetTile(plantPosition, crop.CropSetting.growthStages[stage]);
        }
    }

    public bool TryToHarverst(Vector3Int pos)
    {
        var networkPos = new NetworkVector3Int(pos);

        if (PlantedCropsNetwork.ContainsKey(networkPos) && PlantedCropsNetwork[networkPos].IsFullyGrown())
        {
            var cropCurrentData = PlantedCropsNetwork[networkPos];
            string cropProductName = cropCurrentData.CropProductName.ToString();
            Item NewCropProductInfo = ItemDatabase.Instance.GetItemByName(cropProductName);

            int[] levelArray = new int[NewCropProductInfo.cropLevelImage.Length];
            for(int i = 0; i < levelArray.Length; i++)
            {
                levelArray[i] = i;
            }

            float[] ratioArray = new float[levelArray.Length];
            //default ratio
            for(int i = 0; i < ratioArray.Length; i++)
            {
                if(i==0)
                ratioArray[i] = 100f;
                else if(i == 1)
                    ratioArray[i] = 20f;
                else ratioArray[i] = 0f;
            }

            // Adjust the level ratio based on fertilizer level
            UtilsClass.AdjustRatioByFertilizerLevel(ref ratioArray, cropCurrentData.QualityFertilizedLevel);
            int level = UtilsClass.PickOneByRatio(levelArray, ratioArray);

            Item CropSeedInfo = ItemDatabase.Instance.GetItemByName(cropCurrentData.CropSeedName.ToString());

            UtilsClass.AdjustRatioByFertilizerLevel(ref CropSeedInfo.CropSetting.ratioForEachNum, cropCurrentData.QuantityFertilizedLevel);

            int numOfProduct = UtilsClass.PickOneByRatio(CropSeedInfo.CropSetting.numOfProductCouldDrop, CropSeedInfo.CropSetting.ratioForEachNum);

            ItemWorld crop = new ItemWorld(System.Guid.NewGuid().ToString(), NewCropProductInfo, numOfProduct, pos, level);
            ItemWorldManager.Instance.DropItemIntoWorld(crop, false, false);

            if (cropCurrentData.CanReHarvest)
            {
                TryModifyCrop(pos, cropCurrentData.CropSeedName.ToString(), cropCurrentData.CurrentStage-1);
            }
            else
            {
                RemoveCrop(pos);
            }
            return true;
        }
        return false;
    }

    public void UpdateCropsGrowthTime(int minute)
    {
        foreach (var crop in PlantedCropsNetwork)
        {
            var cropInfo = crop.Value;
            Debug.Log("Current stage " + cropInfo.CurrentStage);
            if (cropInfo.IsDead) continue;
            
            bool isWatered = TileManager.Instance.WateredTilesNetwork.ContainsKey(crop.Key);
            var cropPos = crop.Key.ToVector3Int();
            if (cropInfo.Season != EnviromentalStatusManager.Instance.eStatus.SeasonStatus)
            {
                cropInfo.CurrentStage = 0; // Crop is dead
                cropInfo.NeedChangeStage = true;
            }
            else
            {
                cropInfo.GrowthTimeUpdate(minute, isWatered);
            }
            PlantedCropsNetwork[crop.Key] = cropInfo;
            Debug.Log("crop growth time: " + cropInfo.StageTimeCounter);

            if (cropInfo.NeedChangeStage)
            {
                cropInfo.NeedChangeStage = false;
                PlantedCropsNetwork[crop.Key] = cropInfo;
                TryModifyCrop(cropPos, cropInfo.CropSeedName.ToString(), cropInfo.CurrentStage); // this is change stage so dont need reassign the variable
            }

        }
    }


    public void RemoveCrop(Vector3Int pos)
    {
        TryModifyCrop(pos, null, 0);
    }


    public void LoadData(GameData data)
    {
        if (!IsHost) return;
        //if (SceneManager.GetSceneByName("WorldScene").isLoaded) return;

        //cropTilemap = GameObject.Find("PlantGround").GetComponent<Tilemap>();
        Debug.Log("did subscribe time increase on crop manager");
        PlantedCrops = data.CropsSaveData.CropTiles;
        StartCoroutine(MoveLocalListToNetwork());
        StartCoroutine(ApplyCropTilesOnHostLoad());
        GameEventsManager.Instance.enviromentStatusEvents.onTimeIncrease += UpdateCropsGrowthTime;
    }
    private IEnumerator MoveLocalListToNetwork()
    {

        yield return new WaitUntil(() => IsSpawned);
        foreach (var item in PlantedCrops)
        {
            var networkPos = new NetworkVector3Int(item.Key);
            PlantedCropsNetwork.Add(networkPos, item.Value);
        }
        PlantedCrops.Clear();
    }
    private IEnumerator ApplyCropTilesOnHostLoad()
    {
        yield return new WaitForEndOfFrame();
        foreach (var crop in PlantedCropsNetwork)
        {
            TryModifyCrop(crop.Key.ToVector3Int(), crop.Value.CropSeedName.ToString(), crop.Value.CurrentStage);
        }
    }
    private void MoveNetworkListToLocal()
    {
        PlantedCrops.Clear();
        foreach (var item in PlantedCropsNetwork)
        {
            var localPos = item.Key.ToVector3Int();
            PlantedCrops.Add(localPos, item.Value);
        }
    }
    public void SaveData(ref GameData data)
    {
        if (!IsHost) return;
        GameEventsManager.Instance.enviromentStatusEvents.onTimeIncrease -= UpdateCropsGrowthTime;
        MoveNetworkListToLocal();
        _cropsSaveData.SetCropsData(PlantedCrops);
        data.SetCropsData(_cropsSaveData);
    }

}
