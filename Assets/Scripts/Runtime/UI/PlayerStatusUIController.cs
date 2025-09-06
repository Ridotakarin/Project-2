using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIController : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;
    [SerializeField] private Slider staminaBar;

    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private FloatVariable playerMana;
    [SerializeField] private FloatVariable playerStamina;

    private void Update()
    {
        ChangeHealthValue(playerHealth.Value);
        ChangeManaValue(playerMana.Value);
        ChangeStaminaValue(playerStamina.Value);
    }

    private void ChangeHealthValue(float value)
    {
        healthBar.value = value/100;
    }

    private void ChangeManaValue(float value)
    {
        manaBar.value = value/100;
    }

    private void ChangeStaminaValue(float value)
    {
        staminaBar.value = value/100;
    }
}
