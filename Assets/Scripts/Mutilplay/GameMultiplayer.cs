using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.Services.Authentication;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayerManager : NetworkPersistentSingleton<GameMultiplayerManager>
{
    public static bool playMultiplayer = false;

    [SerializeField] private List<RuntimeAnimatorController> charactersAnimator;

    private string playerName;

    public PlayerDataSO playerDataSO;

    public override void OnNetworkSpawn()
    {
        Debug.Log("GameMultiplayerManager OnNetworkSpawn called");
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        playerDataSO.playerName = playerName;
    }

    public void SetCharacterId(int characterId)
    {
        playerDataSO.characterId = characterId;
    }

    public RuntimeAnimatorController GetCharactersAnimator(int characterId)
    {
        return charactersAnimator[characterId];
    }
}
