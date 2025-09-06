using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class EnviromentalStatusManager : NetworkPersistentSingleton<EnviromentalStatusManager>, IDataPersistence
{
    public EnvironmentalStatus eStatus;

    public static event Action<ESeason> ChangeSeasonEvent;

    //public static event Action<int> OnTimeIncrease;
    public int minutesToIncrease;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Start()
    {
        //DataPersistenceManager.Instance.LoadGame();

        StartCoroutine(WaitToIncreaseDay());
    }

    public bool ChangeSeason()
    {
        switch (eStatus.DateTime.Month, eStatus.DateTime.Day, eStatus.DateTime.Hour, eStatus.DateTime.Minute)
        {
            case (1, 1, 0, 0):
                {
                    eStatus.SetSeasonStatus(ESeason.Spring);
                    return true;
                }
            case (4, 1, 0, 0):
                {
                    eStatus.SetSeasonStatus(ESeason.Summer);
                    return true;
                }
            case (7, 1, 0, 0):
                {
                    eStatus.SetSeasonStatus(ESeason.Autumn);
                    return true;
                }
            case (10, 1, 0, 0):
                {
                    eStatus.SetSeasonStatus(ESeason.Winter);
                    return true;
                }
            default:
                {
                    return false;
                }
        }
    }

    public bool Startday()
    {
        if (eStatus.DateTime.Hour == 6 && eStatus.DateTime.Minute == 0)
        {
            return true;
        }
        return false;
    }

    public bool EndDay()
    {
        if (eStatus.DateTime.Hour == 18 && eStatus.DateTime.Minute == 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator WaitToIncreaseDay()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening) yield return null;

        do
        {
            //DayCycleHandler.Instance.MoveSunAndMoon();
            //DayCycleHandler.Instance.UpdateLight();
            if (ChangeSeason())
            {
                ChangeSeasonEvent?.Invoke(eStatus.SeasonStatus);
            }
            if (Startday())
            {
                GameEventsManager.Instance.npcEvents.SpawnNpc();
            }
            if (EndDay())
            {
                GameEventsManager.Instance.npcEvents.CallNpcHome();
            }
            yield return new WaitForSeconds(1);
            eStatus.IncreaseDate(minutesToIncrease);
            GameEventsManager.Instance.dateTimeEvents.MinuteIncreased(minutesToIncrease);
            GameEventsManager.Instance.dateTimeEvents.DateChanged(eStatus.DateTime);
            //OnTimeIncrease?.Invoke(minutesToIncrease);
        } while (true);
    }

    public void LoadData(GameData gameData)
    {
        if (SceneManager.GetActiveScene().name != Loader.Scene.WorldScene.ToString()) return;
        eStatus = gameData.EnviromentData;
        FishingManager.Instance.ChooseFishesBySeason();
    }

    public void SaveData(ref GameData gameData)
    {
        if (!IsHost) return;

        gameData.SetSeason(eStatus);
    }

}
