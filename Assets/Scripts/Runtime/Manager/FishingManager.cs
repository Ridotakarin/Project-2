using System.Collections.Generic;
using UnityEngine;

public class FishingManager : PersistentSingleton<FishingManager>
{
    [SerializeField]
    private List<Item> springFishes = new List<Item>();

    [SerializeField]
    private List<Item> summerFishes = new List<Item>();

    private List<Item> currentSeasonFishes = new List<Item>();

    public void ChooseFishesBySeason() // choose when enviroment manager loaded save
    {
        currentSeasonFishes.Clear();
        switch(EnviromentalStatusManager.Instance.eStatus.SeasonStatus)
        {
            default:
                {
                    currentSeasonFishes = springFishes;
                    break;
                }
            case ESeason.Summer:
                {
                    currentSeasonFishes = summerFishes;
                    break;
                }
        }
    }

    public Item GetRandomFish()
    {
        if(currentSeasonFishes == null || currentSeasonFishes.Count == 0)
        {
            Debug.LogWarning("No fishes available for the current season.");
            return null;
        }
        int randomIndex = Random.Range(0, currentSeasonFishes.Count);
        return currentSeasonFishes[randomIndex];
    }
}
