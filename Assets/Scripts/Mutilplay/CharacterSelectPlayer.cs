using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour 
{
    [SerializeField] private int playerIndex;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Button selectButton;

    private void Awake() 
    {
        playerAnimator = GetComponent<Animator>();
        selectButton = GetComponent<Button>();
    }

    private void Start() 
    {
        ApplyCharacter();

        selectButton.onClick.AddListener(SetCharacterId);
    }

    private void ApplyCharacter()
    {
        switch (playerIndex)
        {
            case 0:
                playerAnimator.Play("Idle");
                break;
            case 1:
                playerAnimator.Play("Idle_red");
                break;
            case 2:
                playerAnimator.Play("Idle_green");
                break;
            case 3:
                playerAnimator.Play("Idle_blue");
                break;
            default:
                Debug.LogWarning("Invalid player index: " + playerIndex);
                break;
        }
    }

    private void SetCharacterId()
    {
        GameMultiplayerManager.Instance.SetCharacterId(playerIndex);
        GameFlowManager.Instance.Data.SetHasChoosenCharacter(true);
    }
}