using UnityEngine;

public class PlayerHouse : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject areaExit;
    private bool hasOpened = false;
    private bool playerInrange = false;

    private void OnEnable()
    {
        GameEventsManager.Instance.playerHouseEvents.onUnlockHouse += UnlockHouse;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.playerHouseEvents.onUnlockHouse -= UnlockHouse;
    }

    private void Start()
    {
        hasOpened = GameFlowManager.Instance.Data.HasOpenedPlayerHouse;

        CheckOpenedHouse();
    }

    private void CheckOpenedHouse()
    {
        if (hasOpened)
        {
            door.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            areaExit.gameObject.SetActive(true);
            Debug.Log("Player house unlocked!");
        }
        else
        {
            door.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            areaExit.gameObject.SetActive(false);
        }
    }

    private void UnlockHouse()
    {
        if (!playerInrange) return;

        hasOpened = true;
        GameFlowManager.Instance.Data.SetHasOpendPlayerHouse(hasOpened);

        CheckOpenedHouse();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInrange = false;
        }
    }
}
