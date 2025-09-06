using System.Transactions;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIInputNameConfirmButton : MonoBehaviour
{
    public Button confirmButton;
    public TMP_InputField inputField;
    public TextMeshProUGUI errorText;
    public GameObject joinPanel;

    private void Start()
    {
        if (confirmButton == null)
        {
            Debug.LogError("Confirm Button is not assigned in the inspector.");
            return;
        }
        errorText.gameObject.SetActive(false);
        joinPanel.SetActive(false);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    private void OnConfirmButtonClicked()
    {
        if (!GameFlowManager.Instance.Data.HasChoosenCharacter)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Please choose a character first.";
            return;
        }

        if (string.IsNullOrEmpty(inputField.text))
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Please enter a name.";
            return;
        }

        if (inputField.text.Length < 3 || inputField.text.Length > 10)
        {
            errorText.gameObject.SetActive(true);
            errorText.text = "Name must be between 3 and 10 characters.";
            return;
        }

        GameMultiplayerManager.Instance.SetPlayerName(inputField.text);

        if (!GameMultiplayerManager.playMultiplayer)
        {
            if (!GameFlowManager.Instance.Data.CompletedFirstCutscene)
            {
                Loader.Load(Loader.Scene.CutScene);
                NetworkManager.Singleton.StartHost();
            }
            else Loader.Load(Loader.Scene.CutScene);
        }
        else
        {
            errorText.gameObject.SetActive(false);
            joinPanel.SetActive(true);
        }
    }
}
