using System;
using UnityEngine;

public class NetworkObjectEvents
{
    public event Action onNetworkObjectSpawned;
    public void OnNetworkObjectSpawned()
    {
        onNetworkObjectSpawned?.Invoke();
    }
}
