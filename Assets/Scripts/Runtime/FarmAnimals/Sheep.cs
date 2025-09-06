using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : FarmAnimal
{
    [SerializeField] private GameObject sheepPrefab;
    private SheepGrowthStage _currentGrowthStage = 0;


    protected override void MakeProduct()
    {
        canMakeProduct = true;

    }
    [ContextMenu("shave hair")]
    protected override void GetProduct()
    {
        if (canMakeProduct)
        {
            canMakeProduct = false;
            Debug.Log("Got hair");
            DecreaseGrowStage();
        }
    }
    protected override void InteractWithAnimal()
    {

    }

    protected override void ApplyStage(string stage)
    {
        base.ApplyStage(stage);
    }

    public override void FedTimeHandler(int minute)
    {
        if (!isFed) return;
        fedTimeCounter += minute;
        if(_currentGrowthStage != SheepGrowthStage.Haired)
        {
            if (fedTimeCounter >= _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                ChangeResetFedTime();
                IncreaseGrowStage();
            }
        }
        if (fedTimeCounter >= resetFedTime)
        {
            ChangeResetFedTime(resetFedTime + 1000);
            isFed = false;
        }
    }

    public override void IncreaseGrowStage()
    {
        if((int)_currentGrowthStage == 0)
        {
            _currentGrowthStage += 1;
        }
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(SheepGrowthStage)).Length - 1;
        _currentGrowthStage = (SheepGrowthStage)Mathf.Min(next, max);

        base.ApplyStage(_currentGrowthStage.ToString()); 
        isFed = false;
        if (_currentGrowthStage == SheepGrowthStage.Haired)
            MakeProduct();
    }

    private void DecreaseGrowStage()
    {
        int prev = (int)_currentGrowthStage - 1;
        int min = 0;
        _currentGrowthStage = (SheepGrowthStage)Mathf.Max(prev, min);

        base.ApplyStage(_currentGrowthStage.ToString());
        fedTimeCounter = 0;
        ChangeResetFedTime();
    }

    public override void Interact()
    {
        base.Interact();
        _animator.SetTrigger("Interact");
    }

    public void SetSheepGrowthStage(SheepGrowthStage stage)
    {
        _currentGrowthStage = stage;
        base.ApplyStage(_currentGrowthStage.ToString());
    }

    public SheepGrowthStage GetSheepGrowthStage()
    {
        return _currentGrowthStage;
    }
}
