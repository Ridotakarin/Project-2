using UnityEngine;

public class ExplosionProjectile : MonoBehaviour
{
    public LayerMask hitAbleLayer;
    [SerializeField] private float _knockbackStrength;
    [HideInInspector] public float DamageAmount; // set by the person shoot it
    [SerializeField] private GameObject _shadowObject;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & hitAbleLayer) == 0)
            return;

        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (circle == null)
        {
            Debug.LogWarning("No CircleCollider2D found on this object.");
            return;
        }

        float radius = circle.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y); // world size
        Vector2 explosionPosition = transform.position;

        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, radius, hitAbleLayer);

        foreach (var hit in hits)
        {
            Damageable damageable = hit.GetComponent<Damageable>();
            if (damageable != null)
            {
                Vector2 direction = (hit.transform.position - (Vector3)explosionPosition).normalized;
                Vector2 deliveredKnockBack = direction * _knockbackStrength;

                damageable.Hit(DamageAmount, deliveredKnockBack);
            }
        }
    }



    public void DestroyAfterDoneExplosion()
    {
        Destroy(transform.root.gameObject);
        
    }

    public void DestroyShadowObject()
    {
        AudioManager.Instance.PlaySFX("explosion");
        Destroy(_shadowObject);
    }

}
