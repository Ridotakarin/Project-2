using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LetterUI : MonoBehaviour
{
    [SerializeField] private GameObject letterContent;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.uiEvents.onOpenLetter += Open;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.uiEvents.onOpenLetter -= Open;
    }

    public void Open()
    {
        letterContent.gameObject.SetActive(true);
    }

    public void Close()
    {
        GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.Player);
        letterContent.gameObject.SetActive(false);

        if (GameFlowManager.Instance.Data.CompletedFirstCutscene) return;

        GameEventsManager.Instance.dialogueEvents.EnterDialogue("prepare_departure");
    }
}
