using System;
using UnityEngine;

public class ObjectEvents
{
    public event Action onSpawnObject;
    public void SpawnObject()
    {
        onSpawnObject?.Invoke();
    }
}
