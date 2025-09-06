using UnityEngine;

public class AreaTriggerZone : MonoBehaviour
{
    private AreaExit areaExit;

    private void Awake()
    {
        areaExit = GetComponentInParent<AreaExit>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if( collision.CompareTag("Player") && areaExit.UseClickToExit)
        {
            areaExit.CanClick = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && areaExit.UseClickToExit)
        {
            areaExit.CanClick = false; // Reset click state when player exits the trigger
        }
    }
}
