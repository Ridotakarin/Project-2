using UnityEngine;
using UnityEngine.Tilemaps;

public class AttackSetting : MonoBehaviour
{
    [SerializeField] private int attackDamage;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private LayerMask hitAbleLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the collided object is in the hitAbleLayer
        if (((1 << collision.gameObject.layer) & hitAbleLayer) == 0)
            return;

        Damageable damageable = collision.gameObject.GetComponent<Damageable>();

        if (damageable != null)
        {
            GameObject rootObject = transform.root.gameObject;
            Vector3 rootPosition = rootObject.transform.position;

            Vector2 direction = (collision.transform.position - rootPosition).normalized;
            Vector2 deliveredKnockBack = direction * knockbackStrength;

            bool goHit = damageable.Hit(attackDamage, deliveredKnockBack);

            if (goHit)
            {
                Debug.Log(collision.gameObject.name + " hit for " + attackDamage);
            }
        }
        
    }
}
