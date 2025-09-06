using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : PersistentSingleton<GameFlowManager>, IDataPersistence
{
    private GameFlowData _data;
    private bool hasLoaded = false;

    public GameFlowData Data
    { get { return _data; } }

    public void HasChoosenCharacte(bool hasChoosenCharacter)
    {
        _data.SetHasChoosenCharacter(hasChoosenCharacter);
    }

    public void SetCompletedFirstCutscene(bool completed)
    {
        _data.SetCompletedFirstCutscene(completed);
    }

    public void SetCompletedSecondCutscene(bool completed)
    {
        _data.SetCompletedSecondCutscene(completed);
    }

    public void LoadData(GameData data)
    {
        if (SceneManager.GetActiveScene().name == Loader.Scene.MainMenu.ToString()) return;
        if (hasLoaded) return;

        _data = data.GameFlowData;
        hasLoaded = true;
    }

    public void SaveData(ref GameData data)
    {
        data.SetGameFlowData(_data);
    }
}
