using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ShopTrigger : MonoBehaviour
{
    private bool playerInrange = false;

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

    public void OpenShop()
    {
        if (playerInrange)
        {
            GameEventsManager.Instance.shopEvents.OpenShop();
            GameEventsManager.Instance.inputReader.SwitchActionMap(ActionMap.UI);
        }
    }
}
