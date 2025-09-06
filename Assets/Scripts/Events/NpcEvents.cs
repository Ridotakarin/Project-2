using System;
using UnityEngine;

public class NpcEvents
{
    public event Action onSpawnNpc;
    public void SpawnNpc()
    {
        onSpawnNpc?.Invoke();
    }

    public event Action onNpcSpawned;
    public void NpcSpawned()
    {
        onNpcSpawned?.Invoke();
    }

    public event Action onCallNpcHome;
    public void CallNpcHome()
    {
        onCallNpcHome?.Invoke();
    }
}
