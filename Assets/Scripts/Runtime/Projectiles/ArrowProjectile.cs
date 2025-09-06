using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private GameObject arrowImpact;
    [SerializeField] private float tipOffset = 0.16f; // Adjust to match arrow sprite length
    [HideInInspector] public float DamageAmount; // set by the person shoot it
    [SerializeField] private float knockbackStrength;
    public LayerMask hitAbleLayer;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is in the hitAbleLayer
        if (((1 << collision.gameObject.layer) & hitAbleLayer) == 0)
            return;

        // Use velocity to determine tip position
        Vector2 direction = _rb.linearVelocity.normalized;
        Vector2 tipPosition = (Vector2)transform.position + direction * tipOffset;

        // Instantiate impact at the tip
        Instantiate(arrowImpact, tipPosition, transform.rotation);

        if(collision.TryGetComponent<Damageable>(out Damageable damageable))
        {
            // Apply damage to the damageable object
            damageable.Hit(DamageAmount, direction * knockbackStrength); // Assuming 1 is the damage amount

        }
        // Destroy arrow after impact
        Destroy(gameObject);
    }
}
