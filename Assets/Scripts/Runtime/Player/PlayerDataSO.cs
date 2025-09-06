using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Scriptable Objects/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    public int characterId;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;
    public float maxHealth;
    public float currentHealth;
    public float maxMana;
    public float currentMana;
    public float maxStamina;
    public float currentStamina;
    public int money;
    public Vector3 position;
}
