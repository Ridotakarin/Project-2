using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmAnimalManager : PersistentSingleton<FarmAnimalManager>
{
    [SerializeField]
    private List<FarmAnimal> _farmAnimals = new List<FarmAnimal>();

    [SerializeField]
    private GameObject _chickenPrefab;

    [SerializeField]
    private GameObject _femaleCowPrefab;

    [SerializeField]
    private GameObject _maleCowPrefab;

    [SerializeField]
    private GameObject _femaleSheepPrefab;

    [SerializeField]
    private GameObject _maleSheepPrefab;
    private void OnEnable()
    {
        GameEventsManager.Instance.dateTimeEvents.onMinuteIncrease += IncreaseFedTime;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.dateTimeEvents.onMinuteIncrease -= IncreaseFedTime;
    }

    private void IncreaseFedTime(int minute)
    {
        for(int i = _farmAnimals.Count - 1; i >= 0; i--)
        {
            _farmAnimals[i].FedTimeHandler(minute);
            
        }
    }

    public void RegisterAnimal(FarmAnimal animal)
    {
        if (!_farmAnimals.Contains(animal))
            _farmAnimals.Add(animal);
    }

    public void UnregisterAnimal(FarmAnimal animal)
    {
        if (_farmAnimals.Contains(animal))
            _farmAnimals.Remove(animal);
    }

    public void LoadData(GameData data)
    {
        FarmAnimalSaveDataCollection farmAnimalSaveDataCollection = data.FarmAnimalSaveDataCollection;
        if (farmAnimalSaveDataCollection == null || farmAnimalSaveDataCollection.GetFarmAnimalSaveDataList().Count == 0) return;

        foreach (FarmAnimalSaveData farmAnimalSaveData in farmAnimalSaveDataCollection.GetFarmAnimalSaveDataList())
        {
            var farmAnimalTupleData = farmAnimalSaveData.GetData();
            FarmAnimal farmAnimal = SpawnFarmAnimals(farmAnimalTupleData.animalKind).GetComponent<FarmAnimal>();
            farmAnimal.LoadData(farmAnimalSaveData);
            farmAnimal.SetCurrentGrowthStage(farmAnimalSaveData);
        }
        Debug.Log($"Farm animals loaded: {_farmAnimals.Count} animals found in save data.");
        
    }

    private GameObject SpawnFarmAnimals(FarmAnimal.FarmAnimalKind farmAnimalKind)
    {
        switch (farmAnimalKind)
        {
            case FarmAnimal.FarmAnimalKind.Chicken:
                return Instantiate(_chickenPrefab, Vector3.zero, Quaternion.identity);
            case FarmAnimal.FarmAnimalKind.FemaleCow:
                return Instantiate(_femaleCowPrefab, Vector3.zero, Quaternion.identity);
            case FarmAnimal.FarmAnimalKind.MaleCow:
                return Instantiate(_maleCowPrefab, Vector3.zero, Quaternion.identity);
            case FarmAnimal.FarmAnimalKind.FemaleSheep:
                return Instantiate(_femaleSheepPrefab, Vector3.zero, Quaternion.identity);
            case FarmAnimal.FarmAnimalKind.MaleSheep:
                return Instantiate(_maleSheepPrefab, Vector3.zero, Quaternion.identity);
            default:
                return null;
        }
    }

    public void SaveData(ref GameData data)
    {
        FarmAnimalSaveDataCollection farmAnimalSaveDataCollection = new FarmAnimalSaveDataCollection();
        foreach (FarmAnimal farmAnimal in _farmAnimals)
        {
            var farmAnimalSaveData = farmAnimal.GetDataToSave();
            CheckThisTypeToGetCurrentGrowthStage(farmAnimal, farmAnimalSaveData);
            farmAnimalSaveDataCollection.AddFarmAnimalSaveData(farmAnimalSaveData);

        }
        data.SetFarmAnimalSaveDataCollection(farmAnimalSaveDataCollection);
        Debug.Log($"Farm animals saved: {_farmAnimals.Count} animals saved to data.");
    }


    private void CheckThisTypeToGetCurrentGrowthStage(FarmAnimal farmAnimal, FarmAnimalSaveData farmAnimalSaveData)
    {
        switch(farmAnimal)
        {
            case Chicken chicken:
                farmAnimalSaveData.SetChickenGrowthStage(chicken.GetChickenGrowthStage());
                break;
            case Cow cow:
                farmAnimalSaveData.SetCowGrowthStage(cow.GetCowGrowthStage());
                break;
            case Sheep sheep:
                farmAnimalSaveData.SetSheepGrowthStage(sheep.GetSheepGrowthStage());
                break;
        }
    }
}
