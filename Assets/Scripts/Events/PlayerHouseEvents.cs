using System;
using UnityEngine;

public class PlayerHouseEvents
{
    public event Action onUnlockHouse;
    public void UnlockHouse()
    {
        onUnlockHouse?.Invoke();
    }
}
