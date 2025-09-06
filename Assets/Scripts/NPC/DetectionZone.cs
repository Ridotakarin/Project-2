using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public LayerMask layer;
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            if (!detectedColliders.Contains(collision))
            {
                Debug.Log("Add" + collision.gameObject.name);
                detectedColliders.Add(collision);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layer) != 0)
        {
            if (detectedColliders.Contains(collision))
            {
                detectedColliders.Remove(collision);
            }
        }
    }
}
