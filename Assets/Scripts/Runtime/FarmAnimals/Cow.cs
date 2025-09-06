using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : FarmAnimal
{
    [SerializeField] private GameObject cowPrefab;
    private CowGrowthStage _currentGrowthStage = 0;

    protected override void MakeProduct()
    {
        canMakeProduct = true;

    }
    [ContextMenu("get milk")]
    protected override void GetProduct()
    {
        if (canMakeProduct)
        {
            canMakeProduct = false;
            Debug.Log("Got milk");
        }
    }
    public override void FedTimeHandler(int minute)
    {
        if (!isFed) return;
        fedTimeCounter += minute;
        if (_currentGrowthStage == CowGrowthStage.Baby)
        {
            if (fedTimeCounter >= _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                ChangeResetFedTime();
                IncreaseGrowStage();
            }
        }
        else
        {
            if (fedTimeCounter >= _animalInfo.FedTimesNeededToMakeProduct && !canMakeProduct)
            {
                fedTimeCounter = 0;
                ChangeResetFedTime();
                MakeProduct();
            }
        }

        if (fedTimeCounter >= resetFedTime)
        {
            ChangeResetFedTime(resetFedTime + 1000);
            isFed = false;
        }
    }
    protected override void ApplyStage(string stage)
    {
        base.ApplyStage(stage);
    }
    public override void IncreaseGrowStage()
    {
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(CowGrowthStage)).Length - 1;
        _currentGrowthStage = (CowGrowthStage)Mathf.Min(next, max);

        base.ApplyStage(_currentGrowthStage.ToString());
        isFed = false;
    }

    public override void Interact()
    {
        base.Interact();
        _animator.SetTrigger("Interact");
    }

    public void SetCowGrowthStage(CowGrowthStage stage)
    {
        _currentGrowthStage = stage;
        base.ApplyStage(_currentGrowthStage.ToString());
    }

    public CowGrowthStage GetCowGrowthStage()
    {
        return _currentGrowthStage;
    }
}
