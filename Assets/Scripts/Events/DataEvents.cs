using System;
using System.Collections.Generic;
using UnityEngine;

public class DataEvents
{
    public event Action<string> onInitialized;
    public void OnInitialized(string fileName)
    {
        onInitialized?.Invoke(fileName);
    }
    public event Action<string, bool, Action> DataLoaded;
    public void OnDataLoaded(string fileName, bool isNewGame, Action callback)
    {
        DataLoaded?.Invoke(fileName, isNewGame, callback);
    }
    public event Action<bool> DataSaved;
    public void OnDataSaved(bool isNewGame)
    {
        DataSaved?.Invoke(isNewGame);
    }

    public event Action<string> onDataLoading;
    public void OnDataLoading(string data)
    {
        onDataLoading?.Invoke(data);
    }

    public event Action<string> onExitToWorldScene;
    public void OnExitToWorldScene(string sceneName)
    {
        onExitToWorldScene?.Invoke(sceneName);
    }
}
