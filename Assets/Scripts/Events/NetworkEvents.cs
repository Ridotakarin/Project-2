using System;
using UnityEngine;

public class NetworkEvents
{
    public event Action onSessionCreate;
    public void OnSessionCreate()
    {
        onSessionCreate?.Invoke();
    }
}
