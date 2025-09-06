using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/FarmAnimal")]
public class FarmAnimalSO : ScriptableObject
{
    public int FedTimesNeededToGrow;
    public int FedTimesNeededToMakeProduct;
    public Gender Gender;
    public GameObject eggPrefab;
}
public enum Gender
{
    None,
    Male,
    Female
}

public enum ChickenGrowthStage 
{
    Egg,
    Baby,
    Mature
}

public enum CowGrowthStage 
{
    Baby,
    Mature
}

public enum SheepGrowthStage
{
    Baby,
    Shaved,
    Haired,
}


