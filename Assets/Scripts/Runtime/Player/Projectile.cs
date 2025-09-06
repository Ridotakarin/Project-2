using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(3f, 0);
    public Vector2 knockback = new Vector2(0, 0);

    private Rigidbody2D rb;
    private Animator animator;

    [SerializeField] private GameObject trail;

    private bool isHit = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rb.linearVelocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);

        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (isHit)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockBack = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool goHit = damageable.Hit(damage, deliveredKnockBack);

            if (goHit && !isHit)
            {
                Debug.Log(collision.name + " hit for " + damage);
                Destroy(trail);
                animator.SetBool(AnimationStrings.isHit, true);
                isHit = true;
            }

            Destroy(trail);
            animator.SetBool(AnimationStrings.isHit, true);
            isHit = true;
        }
        else
        {
            Destroy(trail);
            animator.SetBool(AnimationStrings.isHit, true);
            isHit = true;
        }
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
