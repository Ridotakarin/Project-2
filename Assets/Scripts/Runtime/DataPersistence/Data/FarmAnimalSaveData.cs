using UnityEngine;
using System.Collections.Generic;
using static FarmAnimal;

[System.Serializable]
public class FarmAnimalSaveData
{
    [SerializeField]
    private FarmAnimalKind animalKind;
    [SerializeField]
    private ChickenGrowthStage chickenGrowthStage;
    [SerializeField]
    private CowGrowthStage cowGrowthStage;
    [SerializeField]
    private SheepGrowthStage sheepGrowthStage;
    [SerializeField]
    private int resetFedTime = 1000;
    [SerializeField]
    private bool canMakeProduct = false;
    [SerializeField]
    private int fedTimeCounter = 0;
    [SerializeField]
    private bool isFed = false;
    [SerializeField]
    private Vector3 position;
    public FarmAnimalSaveData(FarmAnimalKind animalKind, int resetFedTime, bool canMakeProduct, int fedTimeCounter, bool isFed, Vector3 position)
    {
        this.animalKind = animalKind;
        this.resetFedTime = resetFedTime;
        this.canMakeProduct = canMakeProduct;
        this.fedTimeCounter = fedTimeCounter;
        this.isFed = isFed;
        this.position = position;
    }

    public (FarmAnimalKind animalKind, int ResetFedTime, bool CanMakeProduct, int FedTimeCounter, bool IsFed, Vector3 Position) GetData()
    {
        return (animalKind , resetFedTime, canMakeProduct, fedTimeCounter, isFed, position);
    }

    public ChickenGrowthStage GetChickenGrowthStage()
    {
        return chickenGrowthStage;
    }

    public void SetChickenGrowthStage(ChickenGrowthStage stage)
    {
        chickenGrowthStage = stage;
    }

    public CowGrowthStage GetCowGrowthStage()
    {
        return cowGrowthStage;
    }

    public void SetCowGrowthStage(CowGrowthStage stage)
    {
        cowGrowthStage = stage;
    }

    public SheepGrowthStage GetSheepGrowthStage()
    {
        return sheepGrowthStage;
    }

    public void SetSheepGrowthStage(SheepGrowthStage stage)
    {
        sheepGrowthStage = stage;
    }
}

[System.Serializable]
public class FarmAnimalSaveDataCollection
{
    [SerializeField]
    private List<FarmAnimalSaveData> farmAnimalSaveDataList = new List<FarmAnimalSaveData>();

    public FarmAnimalSaveDataCollection()
    {
        farmAnimalSaveDataList = new List<FarmAnimalSaveData>();
    }
    public void AddFarmAnimalSaveData(FarmAnimalSaveData data)
    {
        farmAnimalSaveDataList.Add(data);
    }

    public List<FarmAnimalSaveData> GetFarmAnimalSaveDataList()
    {
        return farmAnimalSaveDataList;
    }

    public void Clear()
    {
        farmAnimalSaveDataList.Clear();
    }
}
