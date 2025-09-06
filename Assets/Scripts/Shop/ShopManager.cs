using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] private List<ShopData> _shopData;

    public void LoadData(GameData gameData)
    {

    }

    public void SaveData(ref GameData gameData)
    {

    }
}
