using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : PersistentSingleton<GoldManager>, IDataPersistence
{
    public int currentGold { get; private set; }

    private void OnEnable()
    {
        GameEventsManager.Instance.goldEvents.onGoldGained += GoldGained;
        GameEventsManager.Instance.goldEvents.onGoldLost += GoldLost;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.goldEvents.onGoldGained -= GoldGained;
        GameEventsManager.Instance.goldEvents.onGoldLost -= GoldLost;
    }

    private void Start()
    {
        GameEventsManager.Instance.goldEvents.GoldChange(currentGold);
    }

    private void GoldGained(int gold)
    {
        currentGold += gold;
        GameEventsManager.Instance.goldEvents.GoldChange(currentGold);
    }

    private void GoldLost(int gold)
    {
        currentGold -= gold;
        GameEventsManager.Instance.goldEvents.GoldChange(currentGold);
    }

    public void LoadData(GameData data)
    {
        currentGold = data.PlayerData.Money;
        GameEventsManager.Instance.goldEvents.GoldChange(currentGold);
    }

    public void SaveData(ref GameData data)
    {
        data.PlayerData.SetMoney(currentGold);
    }
}
