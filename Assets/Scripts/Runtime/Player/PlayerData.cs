using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct PlayerDataNetwork : IEquatable<PlayerDataNetwork>, INetworkSerializable
{
    [NonSerialized] public ulong clientId;
    [SerializeField] public int characterId;
    [SerializeField] public FixedString64Bytes playerName;
    [SerializeField] public FixedString64Bytes playerId;

    public PlayerDataNetwork(ulong clientId, int characterId, FixedString64Bytes playerName, FixedString64Bytes playerId)
    {
        this.clientId = clientId;
        this.characterId = characterId;
        this.playerName = playerName;
        this.playerId = playerId;
    }

    public bool Equals(PlayerDataNetwork other)
    {
        return
            clientId == other.clientId &&
            characterId == other.characterId &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref characterId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}

[System.Serializable]
public class PlayerData
{
    [SerializeField] private PlayerDataNetwork _playerDataNetwork;
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _maxMana;
    [SerializeField] private float _currentMana;
    [SerializeField] private float _maxStamina;
    [SerializeField] private float _currentStamina;
    [SerializeField] private int _money;
    [SerializeField] private Vector3 _position;

    public PlayerDataNetwork PlayerDataNetwork
    { get { return _playerDataNetwork; } }

    public float MaxHealth
    { get { return _maxHealth; } } 

    public float CurrentHealth 
    { get { return _currentHealth; } }

    public float MaxMana
    { get { return _maxMana; } }

    public float CurrentMana
    { get { return _currentMana; } }

    public float MaxStamina
    { get { return _maxStamina; } }

    public float CurrentStamina
    { get { return _currentStamina; } }

    public int Money
    { get { return _money; } }

    public Vector3 Position
    { get { return _position; } }

    public PlayerData()
    {
        this._playerDataNetwork = new PlayerDataNetwork
        {
            clientId = 0,
            characterId = 0,
            playerName = "Player",
            playerId = "PlayerID"
        };
        this._maxHealth = 100;
        this._currentHealth = 100;
        this._maxMana = 100;
        this._currentMana = 100;
        this._maxStamina = 100;
        this._currentStamina = 100;
        this._money = 2000;
        this._position = new Vector3(4.371f, 1.154f, 0);
    }

    public PlayerData(PlayerDataNetwork playerDataNetwork, float maxHealth, float currentHealth, float maxMana, float currentMana, float maxStamina, float currentStamina, int money, Vector3 position)
    {
        this._playerDataNetwork = playerDataNetwork;
        this._maxHealth = maxHealth;
        this._currentHealth = currentHealth;
        this._maxMana = maxMana;
        this._currentMana = currentMana;
        this._maxStamina = maxStamina;
        this._currentStamina = currentStamina;
        this._money = money;
        this._position = position;
    }

    // Health
    public void SetMaxHealth(float health)
    {
        this._maxHealth = health;
    }
    // Current health
    public void SetCurrentHealth(float health)
    {
        this._currentHealth = health;
    }

    // Money
    public void SetMoney(int money)
    {
        this._money = money;
    }

    // Position
    public void SetPosition(Vector3 position)
    {
        this._position = position;
    }

    public void SetPlayerDataNetwork(PlayerDataNetwork playerDataNetwork)
    {
        this._playerDataNetwork = playerDataNetwork;
    }
}
