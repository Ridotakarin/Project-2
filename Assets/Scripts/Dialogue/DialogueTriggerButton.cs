using UnityEngine;

public class DialogueTriggerButton : MonoBehaviour
{
    [Header("Ink Name")]
    [SerializeField] private string dialogueKnotName;

    [SerializeField] private GameObject emoteGO;
    private bool playerInRange = false;
    private bool hasEnteredDialogue = false;

    private void Start()
    {
        if (emoteGO != null)
        {
            emoteGO.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Emote GameObject is not assigned.");
        }
    }

    public void EnterDialogue(Component sender, object data)
    {
        if (!playerInRange) return;

        if (hasEnteredDialogue) return;

        GameEventsManager.Instance.dialogueEvents.EnterDialogue(dialogueKnotName);
        if (!hasEnteredDialogue)
        {
            hasEnteredDialogue = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasEnteredDialogue) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (emoteGO != null)
            {
                emoteGO.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hasEnteredDialogue) return;

        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (emoteGO != null)
            {
                emoteGO.SetActive(false);
            }
        }
    }
}
