using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : FarmAnimal
{
    [SerializeField] private ChickenGrowthStage _currentGrowthStage = 0;
    [SerializeField] private bool isBabyDefault = false;

    protected override void Start()
    {
        base.Start();

        if (isBabyDefault) IncreaseGrowStage();
    }

    public override void FedTimeHandler(int minute)
    {
        Debug.Log("fedTimeCounter: " + minute);

        if (_currentGrowthStage == 0)
        {
            fedTimeCounter += minute;
            if (fedTimeCounter >= _animalInfo.FedTimesNeededToGrow)
            {
                fedTimeCounter = 0;
                _animator.Play("AboutToHatch");
            }
        }
        else
        {
            if (!isFed) return;
            fedTimeCounter += minute;
            if (_currentGrowthStage != ChickenGrowthStage.Mature)
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
                if (fedTimeCounter >= _animalInfo.FedTimesNeededToMakeProduct)
                {
                    fedTimeCounter = 0;
                    ChangeResetFedTime();
                    MakeProduct();
                }
            }
            
            if(fedTimeCounter >= resetFedTime)
            {
                ChangeResetFedTime(resetFedTime + 1000);
                isFed = false;
            }
        }
        
    }

    public override bool Eat()
    {
        if (_currentGrowthStage == ChickenGrowthStage.Egg)
            return false;

        return base.Eat();
    }
    public override void Interact()
    {
        if(_currentGrowthStage == ChickenGrowthStage.Egg)
            return;
        base.Interact();
    }
    public override void IncreaseGrowStage()
    {
        int next = (int)_currentGrowthStage + 1;
        int max = System.Enum.GetValues(typeof(ChickenGrowthStage)).Length - 1;
        _currentGrowthStage = (ChickenGrowthStage)Mathf.Min(next, max);

        if((int)_currentGrowthStage == 1)
        {
            GetComponent<Collider2D>().isTrigger = false;
            CanMove = true;
            fedTimeCounter = 0;
        }
        Debug.Log("chicken grow stage: " + _currentGrowthStage.ToString());
        base.ApplyStage(_currentGrowthStage.ToString());
        isFed = false;
    }


    protected override void MakeProduct()
    {
        var newAnimal = Instantiate(_animalInfo.eggPrefab, transform.position, Quaternion.identity);
    }

    protected override IEnumerator PlaySoundAfterAFewTimes()
    {
        yield return new WaitUntil(() => _currentGrowthStage != ChickenGrowthStage.Egg);
        base.PlaySoundAfterAFewTimes();
    }


    public void SetChickenGrowthStage(ChickenGrowthStage stage)
    {
        _currentGrowthStage = stage;
        base.ApplyStage(_currentGrowthStage.ToString());
        if (stage != 0)
        {
            GetComponent<Collider2D>().isTrigger = false;
            CanMove = true;
        }
    }

    public ChickenGrowthStage GetChickenGrowthStage()
    {
        return _currentGrowthStage;
    }
}
