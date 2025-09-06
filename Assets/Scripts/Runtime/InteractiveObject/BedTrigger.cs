using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    private BedScript _bedScript;

    private void Awake()
    {
        _bedScript = GetComponentInParent<BedScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bed trigger");
        if (collision.CompareTag("Player"))
        {
            if (_bedScript.IsBeingUsed) return;
            _bedScript.playerController = collision.GetComponent<PlayerController>();
            _bedScript.playerController.SetCurrentBed(_bedScript);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _bedScript.playerController.ClearBed();
            _bedScript.playerController = null;
        }
    }
}
